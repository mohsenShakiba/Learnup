using Learnup.Application.Authentication;
using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Learnup.Domain.AggregateRoots.Conversations;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

public sealed record ChatWithAi(int? ConversationId, string Message) : IRequest<ChatResponse>;

internal sealed class ChatWithAiHandler(
    IAiService aiService,
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<ChatWithAi, ChatResponse>
{
    public async Task<ChatResponse> Handle(ChatWithAi request, CancellationToken cancellationToken)
    {
        var message = request.Message.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message is required.", nameof(request.Message));
        }

        var userId = identityProvider.UserId;

        var conversation = await ResolveConversationAsync(request.ConversationId, userId, cancellationToken);

        var displayName = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);

        var history = conversation.Messages.OrderBy(m => m.Id).ToList();
        var proxyMessages = ChatSupport.BuildMessages(displayName, history, message);

        conversation.AddMessage(ChatRole.User, message);

        var completion = await aiService.CompleteAsync(proxyMessages, cancellationToken);

        conversation.AddMessage(ChatRole.Assistant, completion.Content, completion.Usage.CompletionTokens);

        await ChatSupport.RecordTokenUsageAsync(dbContext, userId, completion.Usage, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ChatResponse(conversation.Id, completion.Content, completion.Usage.TotalTokens);
    }

    private async Task<Conversation> ResolveConversationAsync(
        int? conversationId,
        int userId,
        CancellationToken cancellationToken)
    {
        if (conversationId is not int id)
        {
            var conversation = new Conversation(userId);
            dbContext.Conversations.Add(conversation);
            return conversation;
        }

        return await dbContext.Conversations
                   .Include(c => c.Messages)
                   .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken)
               ?? throw new KeyNotFoundException($"Conversation {id} was not found.");
    }
}
