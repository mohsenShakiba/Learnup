using Learnup.Application.Features.Public.Ai;
using Learnup.Application.Mediation;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class AiController(IMediator mediator) : BasePublicController
{
    [HttpPost("send")]
    public async Task<ActionResult<string>> Send(
        [FromBody] string prompt,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SendAiText(prompt), cancellationToken);
        return Ok(result);
    }
}
