using Learnup.Application.Authentication;
using Learnup.Application.Exceptions;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Learnup.Application.Services;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

public sealed record ChatWithAi(int? ChatId, string Message) : IRequest<ChatQueuedResponse>;

internal sealed class ChatWithAiHandler(
    IChatCompletionQueue chatCompletionQueue,
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<ChatWithAi, ChatQueuedResponse>
{
    public async Task<ChatQueuedResponse> Handle(ChatWithAi request, CancellationToken cancellationToken)
    {
        var message = request.Message.Trim();
        if (message.Length == 0)
        {
            throw new ArgumentException("Message is required.", nameof(request));
        }

        var tokenUsage = await dbContext.UserTokenUsages
            .FirstOrDefaultAsync(entry => entry.UserId == identityProvider.UserId, cancellationToken);

        if (tokenUsage is null)
        {
            tokenUsage = new UserTokenUsage(identityProvider.UserId);
            dbContext.UserTokenUsages.Add(tokenUsage);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (tokenUsage.TotalTokens >= tokenUsage.AvailableTokens)
        {
            throw new TokenUsageExceedException();
        }

        await chatCompletionQueue.QueueAsync(
            new ChatCompletionJob(identityProvider.UserId, request.ChatId, message),
            cancellationToken);

        return new ChatQueuedResponse(request.ChatId, "Queued");
    }
}
