using Learnup.Application.Features.Public.Ai;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Ai;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class ConversationsController(IMediator mediator) : BasePublicController
{
    [HttpPost(Name = "StartConversation")]
    public async Task<ActionResult<ConversationResponse>> Start(CancellationToken cancellationToken)
    {
        var conversation = await mediator.Send(new StartConversation(), cancellationToken);
        return Ok(conversation);
    }

    [HttpGet(Name = "ListConversations")]
    public async Task<ActionResult<List<ConversationResponse>>> List(CancellationToken cancellationToken)
    {
        var conversations = await mediator.Send(new ListConversations(), cancellationToken);
        return Ok(conversations);
    }

    [HttpGet("{id:int}", Name = "GetConversation")]
    public async Task<ActionResult<ConversationDetailResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var conversation = await mediator.Send(new GetConversation(id), cancellationToken);

        return conversation is null
            ? NotFound()
            : Ok(conversation);
    }
}
