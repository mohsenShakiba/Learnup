using Learnup.Application.Features.Public.Tests;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Tests;
using Learnup.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class GrammarTestsController(IMediator mediator) : BasePublicController
{
    [HttpGet("lesson/{lessonId}", Name = "GetGrammarTests")]
    public async Task<ActionResult<IReadOnlyList<GrammarTestResponse>>> GetByLesson(
        int lessonId,
        CancellationToken cancellationToken)
    {
        var tests = await mediator.Send(new GetGrammarTests(lessonId), cancellationToken);
        return Ok(tests);
    }

    [HttpPost("{id}/answer", Name = "AnswerGrammarTest")]
    public async Task<ActionResult<AnswerTestResponse>> Answer(
        int id,
        [FromBody] AnswerTestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AnswerGrammarTest(id, request.SelectedOptionId), cancellationToken);
        return Ok(result);
    }
}
