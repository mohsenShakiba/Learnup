using Learnup.Application.Features.Public.Ai;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Ai;
using Learnup.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class AiController(IMediator mediator) : BasePublicController
{
    [HttpPost("Process")]
    public async Task<ActionResult<SendAiTextResponse>> Send(
        [FromBody] SendAiTextRequest request,
        CancellationToken cancellationToken)
    {
        var query = new AiTranslate(request.Word, request.Sentence);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("Chat", Name = "ChatWithAi")]
    public async Task<ActionResult<ChatResponse>> Chat(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message is required.");
        }

        var command = new ChatWithAi(request.ConversationId, request.Message);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
