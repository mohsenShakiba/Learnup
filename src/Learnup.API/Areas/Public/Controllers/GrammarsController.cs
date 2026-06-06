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
}
