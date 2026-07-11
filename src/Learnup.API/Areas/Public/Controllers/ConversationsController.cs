using Learnup.Application.Features.Public.Conversations;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Conversations;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class ConversationsController(IMediator mediator) : BasePublicController
{
    [HttpGet("{id:int}", Name = "GetConversationById")]
    public async Task<ActionResult<ConversationResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var conversation = await mediator.Send(new GetConversationById(id), cancellationToken);

        return conversation is null
            ? NotFound()
            : Ok(conversation);
    }

    [HttpGet("{id:int}/items/{itemId:int}/expressions", Name = "GetConversationItemExpressions")]
    public async Task<ActionResult<IReadOnlyList<ConversationItemExpressionResponse>>> GetItemExpressions(
        int id,
        int itemId,
        CancellationToken cancellationToken)
    {
        var expressions = await mediator.Send(new GetConversationItemExpressions(id, itemId), cancellationToken);

        return expressions is null
            ? NotFound()
            : Ok(expressions);
    }
}
