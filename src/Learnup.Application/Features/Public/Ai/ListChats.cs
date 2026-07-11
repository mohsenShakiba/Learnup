using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

public sealed record ListChats : IRequest<List<ChatSummaryResponse>>;

internal sealed class ListChatsHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<ListChats, List<ChatSummaryResponse>>
{
    public async Task<List<ChatSummaryResponse>> Handle(ListChats request, CancellationToken cancellationToken)
    {
        var userId = identityProvider.UserId;

        return await dbContext.Chats
            .AsNoTracking()
            .Where(chat => chat.UserId == userId)
            .OrderByDescending(chat => chat.UpdatedAt)
            .Select(chat => new ChatSummaryResponse(
                chat.Id,
                chat.Title,
                chat.CreatedAt,
                chat.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
