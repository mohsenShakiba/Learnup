using System.Runtime.CompilerServices;
using System.Text;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Conversations;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

internal sealed class ChatStreamService(
    IAiService aiService,
    ILearnupDbContext dbContext)
    : IChatStreamService
{
    public async IAsyncEnumerable<string> StreamAsync(
        int userId,
        int conversationId,
        string message,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var trimmed = message.Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new ArgumentException("Message is required.", nameof(message));
        }

        var conversation = await dbContext.Conversations
                               .Include(c => c.Messages)
                               .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId, cancellationToken)
                           ?? throw new KeyNotFoundException($"Conversation {conversationId} was not found.");

        var displayName = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);

        var history = conversation.Messages.OrderBy(m => m.Id).ToList();
        var proxyMessages = ChatSupport.BuildMessages(displayName, history, trimmed);

        // Persist the user's message up front so it isn't lost if the client disconnects mid-stream.
        conversation.AddMessage(ChatRole.User, trimmed);
        await dbContext.SaveChangesAsync(cancellationToken);

        var builder = new StringBuilder();
        AiTokenUsage? usage = null;

        await foreach (var chunk in aiService.StreamAsync(proxyMessages, cancellationToken))
        {
            if (chunk.Usage is not null)
            {
                usage = chunk.Usage;
            }

            if (!string.IsNullOrEmpty(chunk.ContentDelta))
            {
                builder.Append(chunk.ContentDelta);
                yield return chunk.ContentDelta;
            }
        }

        var reply = builder.ToString();
        var resolvedUsage = usage ?? AiTokenUsage.Empty;

        conversation.AddMessage(ChatRole.Assistant, reply, resolvedUsage.CompletionTokens);
        await ChatSupport.RecordTokenUsageAsync(dbContext, userId, resolvedUsage, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
