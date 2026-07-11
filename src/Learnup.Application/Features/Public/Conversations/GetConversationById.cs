using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Conversations;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Conversations;

public sealed record GetConversationById(int Id) : IRequest<ConversationResponse?>;

internal sealed class GetConversationByIdHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetConversationById, ConversationResponse?>
{
    public async Task<ConversationResponse?> Handle(
        GetConversationById request,
        CancellationToken cancellationToken)
    {
        var conversation = await dbContext.Conversations
            .AsNoTracking()
            .Include(conversation => conversation.Items)
            .FirstOrDefaultAsync(conversation => conversation.Id == request.Id, cancellationToken);

        return conversation?.ToResponse();
    }
}
