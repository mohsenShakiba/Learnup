using Learnup.Application.Authentication;
using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Learnup.Domain.AggregateRoots.Chats;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

public sealed record ChatWithAi(int? ChatId, string Message) : IRequest<ChatResponse>;

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

        var chat = await ResolveChatAsync(request.ChatId, userId, cancellationToken);

        var displayName = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);

        var history = chat.Messages.OrderBy(m => m.Id).ToList();
        var proxyMessages = ChatSupport.BuildMessages(displayName, history, message);

        chat.AddMessage(ChatRole.User, message);

        var completion = await aiService.CompleteAsync(proxyMessages, cancellationToken);

        chat.AddMessage(ChatRole.Assistant, completion.Content, completion.Usage.CompletionTokens);

        await ChatSupport.RecordTokenUsageAsync(dbContext, userId, completion.Usage, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ChatResponse(chat.Id, completion.Content, completion.Usage.TotalTokens);
    }

    private async Task<Chat> ResolveChatAsync(
        int? chatId,
        int userId,
        CancellationToken cancellationToken)
    {
        if (chatId is not int id)
        {
            var chat = new Chat(userId);
            dbContext.Chats.Add(chat);
            return chat;
        }

        return await dbContext.Chats
                   .Include(c => c.Messages)
                   .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken)
               ?? throw new KeyNotFoundException($"Chat {id} was not found.");
    }
}
