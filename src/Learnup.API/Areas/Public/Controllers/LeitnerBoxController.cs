using Learnup.Application.Features.Public.LeitnerBoxes;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.LeitnerBox;
using Learnup.API.Requests;
using Learnup.Application.Requests.Admin.Leitner;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class LeitnerBoxController(IMediator mediator) : BasePublicController
{
    [HttpGet(Name = "GetLeitnerBox")]
    public async Task<ActionResult<LeitnerBoxResponse>> Get(CancellationToken cancellationToken)
    {
        var query = new GetLeitnerBox();
        var box = await mediator.Send(query, cancellationToken);
        return box is null ? NotFound() : Ok(box);
    }

    [HttpPost("vocab/{vocabId}", Name = "AddVocabToLeitnerBox")]
    public async Task<IActionResult> AddVocab(int vocabId, CancellationToken cancellationToken)
    {
        var command = new AddVocabToLeitnerBox(vocabId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("box-level", Name = "GetBoxLevelsInfo")]
    public async Task<BoxLevelResponse> GetBoxLevelsInfo(CancellationToken cancellationToken)
    {
        var query = new GetBoxLevelsInfo();
        var response = await mediator.Send(query, cancellationToken);
        return response;
    }

    [HttpGet("box-level/{id:int}", Name = "GetDueWordsByBoxLevelId")]
    public async Task<ActionResult<IReadOnlyList<DueLeitnerBoxItemResponse>>> GetDueWordsByBoxLevelId(
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetDueWordsByBoxLevelId(id);
        var response = await mediator.Send(query, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost("box-level/review-interval/{boxId:int}", Name = "UpdateBoxLevelReviewIntervals")]
    public async Task<IActionResult> UpdateBoxLevelReviewIntervals(
        int boxId,
        List<UpdateBoxLevelReviewIntervalRequest> request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBoxLevelReviewIntervals(boxId, request);

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}