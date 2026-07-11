using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Learnup.Domain.AggregateRoots.Chats;

namespace Learnup.Application.Features.Public.Ai;

public sealed record StartChat : IRequest<ChatSummaryResponse>;

internal sealed class StartChatHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<StartChat, ChatSummaryResponse>
{
    public async Task<ChatSummaryResponse> Handle(StartChat request, CancellationToken cancellationToken)
    {
        var userId = identityProvider.UserId;

        var chat = new Chat(userId);

        dbContext.Chats.Add(chat);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ChatSummaryResponse(
            chat.Id,
            chat.Title,
            chat.CreatedAt,
            chat.UpdatedAt);
    }
}
