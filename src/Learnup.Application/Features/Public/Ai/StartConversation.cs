using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Learnup.Domain.AggregateRoots.Conversations;

namespace Learnup.Application.Features.Public.Ai;

public sealed record StartConversation : IRequest<ConversationResponse>;

internal sealed class StartConversationHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<StartConversation, ConversationResponse>
{
    public async Task<ConversationResponse> Handle(StartConversation request, CancellationToken cancellationToken)
    {
        var conversation = new Conversation(identityProvider.UserId);

        dbContext.Conversations.Add(conversation);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ConversationResponse(
            conversation.Id,
            conversation.Title,
            conversation.CreatedAt,
            conversation.UpdatedAt);
    }
}
