using Learnup.Application.Features.Public.Grammars;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Grammars;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class GrammarsController(IMediator mediator) : BasePublicController
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GrammarResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var grammar = await mediator.Send(new GetGrammarById(id), cancellationToken);

        return grammar is null
            ? NotFound()
            : Ok(grammar);
    }

    [HttpGet]
    public async Task<ActionResult<List<GrammarResponse>>> ListGrammars()
    {
        var query = new ListGrammars();
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpDelete("{grammarId:int}")]
    public async Task<ActionResult> DeleteGrammar(int grammarId)
    {
        var command = new DeleteGrammar(grammarId);
        await mediator.Send(command);
        return Ok();
    }

    [HttpGet("lessons")]
    public async Task<ActionResult<List<GrammarLessonResponse>>> ListLessons()
    {
        var query = new ListLessons();
        var response = await mediator.Send(query);
        return Ok(response);
    }
}
