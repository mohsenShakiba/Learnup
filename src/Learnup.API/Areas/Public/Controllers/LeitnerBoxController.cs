using Learnup.Application.Features.Public.LeitnerBoxes;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.LeitnerBox;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class LeitnerBoxController(IMediator mediator) : BasePublicController
{
    [HttpGet(Name = "GetLeitnerBox")]
    public async Task<ActionResult<LeitnerBoxResponse>> Get(CancellationToken cancellationToken)
    {
        var box = await mediator.Send(new GetLeitnerBox(), cancellationToken);
        return box is null ? NotFound() : Ok(box);
    }

    [HttpPost("vocab/{vocabId}", Name = "AddVocabToLeitnerBox")]
    public async Task<IActionResult> AddVocab(int vocabId, CancellationToken cancellationToken)
    {
        await mediator.Send(new AddVocabToLeitnerBox(vocabId), cancellationToken);
        return NoContent();
    }

    [HttpPost("box-level", Name = "GetBoxLevelsInfo")]
    public async Task<BoxLevelResponse> GetBoxLevelsInfo(CancellationToken cancellationToken)
    {
        var query = new GetBoxLevelsInfo();
        var response = await mediator.Send(query, cancellationToken);
        return response;
    }
    
}
