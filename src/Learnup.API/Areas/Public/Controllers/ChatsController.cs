using Learnup.Application.Features.Public.Ai;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Ai;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class ChatsController(IMediator mediator) : BasePublicController
{
    [HttpPost(Name = "StartChat")]
    public async Task<ActionResult<ChatSummaryResponse>> Start(CancellationToken cancellationToken)
    {
        var chat = await mediator.Send(new StartChat(), cancellationToken);
        return Ok(chat);
    }

    [HttpGet(Name = "ListChats")]
    public async Task<ActionResult<List<ChatSummaryResponse>>> List(CancellationToken cancellationToken)
    {
        var chats = await mediator.Send(new ListChats(), cancellationToken);
        return Ok(chats);
    }

    [HttpGet("{id:int}", Name = "GetChat")]
    public async Task<ActionResult<ChatDetailResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var chat = await mediator.Send(new GetChat(id), cancellationToken);

        return chat is null
            ? NotFound()
            : Ok(chat);
    }
}
