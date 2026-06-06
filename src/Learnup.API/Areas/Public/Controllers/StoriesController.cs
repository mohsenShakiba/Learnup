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
}
