using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Conversations;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Conversations;

public sealed record GetConversationItemExpressions(int ConversationId, int ConversationItemId)
    : IRequest<IReadOnlyList<ConversationItemExpressionResponse>?>;

internal sealed class GetConversationItemExpressionsHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetConversationItemExpressions, IReadOnlyList<ConversationItemExpressionResponse>?>
{
    public async Task<IReadOnlyList<ConversationItemExpressionResponse>?> Handle(
        GetConversationItemExpressions request,
        CancellationToken cancellationToken)
    {
        var conversationItem = await dbContext.Conversations
            .AsNoTracking()
            .Where(conversation => conversation.Id == request.ConversationId)
            .SelectMany(conversation => conversation.Items)
            .Include(item => item.Expressions)
            .FirstOrDefaultAsync(item => item.Id == request.ConversationItemId, cancellationToken);

        return conversationItem?.Expressions
            .Select(expression => expression.ToResponse())
            .ToArray();
    }
}
