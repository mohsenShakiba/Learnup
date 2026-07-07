using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

public sealed record ListConversations : IRequest<List<ConversationResponse>>;

internal sealed class ListConversationsHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<ListConversations, List<ConversationResponse>>
{
    public async Task<List<ConversationResponse>> Handle(ListConversations request, CancellationToken cancellationToken)
    {
        var userId = identityProvider.UserId;

        return await dbContext.Conversations
            .AsNoTracking()
            .Where(conversation => conversation.UserId == userId)
            .OrderByDescending(conversation => conversation.UpdatedAt)
            .Select(conversation => new ConversationResponse(
                conversation.Id,
                conversation.Title,
                conversation.CreatedAt,
                conversation.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
