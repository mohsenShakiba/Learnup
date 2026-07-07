using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

public sealed record GetConversation(int ConversationId) : IRequest<ConversationDetailResponse?>;

internal sealed class GetConversationHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<GetConversation, ConversationDetailResponse?>
{
    public async Task<ConversationDetailResponse?> Handle(GetConversation request, CancellationToken cancellationToken)
    {
        var userId = identityProvider.UserId;

        return await dbContext.Conversations
            .AsNoTracking()
            .Where(conversation => conversation.Id == request.ConversationId && conversation.UserId == userId)
            .Select(conversation => new ConversationDetailResponse(
                conversation.Id,
                conversation.Title,
                conversation.CreatedAt,
                conversation.UpdatedAt,
                conversation.Messages
                    .OrderBy(message => message.Id)
                    .Select(message => new ChatMessageResponse(
                        message.Id,
                        message.Role.ToString(),
                        message.Content,
                        message.CreatedAt))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
