using Learnup.Application.Features.Public.Tests;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Tests;
using Learnup.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class VocabTestsController(IMediator mediator) : BasePublicController
{
    [HttpGet("lesson/{lessonId}", Name = "GetVocabTests")]
    public async Task<ActionResult<IReadOnlyList<VocabTestResponse>>> GetByLesson(
        int lessonId,
        CancellationToken cancellationToken)
    {
        var tests = await mediator.Send(new GetVocabTests(lessonId), cancellationToken);
        return Ok(tests);
    }

    [HttpPost("{id}/answer", Name = "AnswerVocabTest")]
    public async Task<ActionResult<AnswerTestResponse>> Answer(
        int id,
        [FromBody] AnswerTestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AnswerVocabTest(id, request.SelectedOptionId), cancellationToken);
        return Ok(result);
    }
}
