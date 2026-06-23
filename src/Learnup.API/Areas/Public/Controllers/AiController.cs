using Learnup.Application.Features.Public.Ai;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Ai;
using Learnup.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class AiController(IMediator mediator) : BasePublicController
{
    [HttpPost("send")]
    public async Task<ActionResult<SendAiTextResponse>> Send(
        [FromBody] SendAiTextRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SendAiText(request.Word, request.Sentence), cancellationToken);
        return Ok(result);
    }
}
