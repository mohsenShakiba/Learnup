using Learnup.API.Requests;
using Learnup.Application.Features.Public.Tests;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Tests;
using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class TestsController(IMediator mediator) : BasePublicController
{
    [HttpGet("lesson/{lessonId}", Name = "GetTests")]
    public async Task<ActionResult<IReadOnlyList<TestResponse>>> GetByLesson(
        int lessonId,
        [FromQuery] TestType type,
        CancellationToken cancellationToken)
    {
        var tests = await mediator.Send(new GetTests(lessonId, type), cancellationToken);
        return Ok(tests);
    }

    [HttpPost("{id}/answer", Name = "AnswerTest")]
    public async Task<ActionResult<AnswerTestResponse>> Answer(
        int id,
        [FromBody] AnswerTestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AnswerTest(id, request.SelectedOptionId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("lesson/{lessonId}/reset", Name = "ResetTestResult")]
    public async Task<IActionResult> Reset(
        int lessonId,
        [FromQuery] TestType type,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new ResetTestResult(lessonId, type), cancellationToken);
        return NoContent();
    }

    [HttpPost("lesson/{lessonId}/check", Name = "CheckTest")]
    public async Task<IActionResult> Check(
        int lessonId,
        [FromQuery] TestType type,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new CheckTest(lessonId, type), cancellationToken);
        return NoContent();
    }
}
