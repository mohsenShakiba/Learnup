using Learnup.API.Requests;
using Learnup.Application.Exceptions;
using Learnup.Application.Features.Public.Ai;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Ai;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class ChatsController(
    IMediator mediator) : BasePublicController
{
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
    
    [HttpPost("Process")]
    public async Task<ActionResult<SendAiTextResponse>> Send(
        [FromBody] SendAiTextRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new AiTranslate(request.Word, request.Sentence);
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (TokenUsageExceedException)
        {
            return BadRequest("TokenExceed");
        }
    }

    [HttpPost("Chat", Name = "ChatWithAi")]
    public async Task<ActionResult<ChatQueuedResponse>> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(new ChatWithAi(request.ChatId, request.Message), cancellationToken);
            return Accepted(result);
        }
        catch (TokenUsageExceedException)
        {
            return BadRequest("TokenExceed");
        }
    }
    
    [HttpGet("TokenUsage", Name = "GetAvailableTokenUsage")]
    public async Task<ActionResult<TokenUsageResponse>> GetTokenUsage(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailableTokenUsage(), cancellationToken);
        return Ok(result);
    }
}
