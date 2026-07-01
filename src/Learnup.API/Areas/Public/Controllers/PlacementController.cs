using Learnup.API.Requests;
using Learnup.Application.Features.Public.Placement;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Placement;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class PlacementController(IMediator mediator) : BasePublicController
{
    [HttpGet(Name = "GetPlacementTest")]
    public async Task<ActionResult<PlacementTestResponse>> GetTest(CancellationToken cancellationToken)
    {
        var test = await mediator.Send(new GetPlacementTest(), cancellationToken);

        return test is null
            ? NotFound()
            : Ok(test);
    }

    [HttpPost("submit", Name = "SubmitPlacementTest")]
    public async Task<IActionResult> Submit(
        [FromBody] SubmitPlacementRequest request,
        CancellationToken cancellationToken)
    {
        var answers = request.Answers
            .Select(answer => new PlacementSubmissionAnswer(answer.QuestionId, answer.SelectedOptionId))
            .ToList();

        await mediator.Send(new SubmitPlacementTest(answers), cancellationToken);

        return NoContent();
    }

    [HttpGet("result", Name = "GetPlacementResult")]
    public async Task<ActionResult<PlacementResultResponse>> GetResult(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPlacementResult(), cancellationToken);

        return result is null
            ? NoContent()
            : Ok(result);
    }
}
