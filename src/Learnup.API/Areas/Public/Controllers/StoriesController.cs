using Learnup.Application.Features.Public.Stories;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Stories;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class StoriesController(IMediator mediator) : BasePublicController
{
    [HttpGet("{id:int}", Name = "GetStoryById")]
    public async Task<ActionResult<StoryResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var story = await mediator.Send(new GetStoryById(id), cancellationToken);

        return story is null
            ? NotFound()
            : Ok(story);
    }

    [HttpGet("{id:int}/items/{itemId:int}/expressions", Name = "GetStoryItemExpressions")]
    public async Task<ActionResult<IReadOnlyList<StoryItemExpressionResponse>>> GetItemExpressions(
        int id,
        int itemId,
        CancellationToken cancellationToken)
    {
        var expressions = await mediator.Send(new GetStoryItemExpressions(id, itemId), cancellationToken);

        return expressions is null
            ? NotFound()
            : Ok(expressions);
    }
}
