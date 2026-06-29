using Learnup.Application.Features.Public.Lessons;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Lessons;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class LessonsController(IMediator mediator) : BasePublicController
{
    [HttpGet("course/{courseId:int}", Name = "GetLessonsByCourseId")]
    public async Task<ActionResult<IReadOnlyList<LessonResponse>>> GetByCourseId(
        int courseId,
        CancellationToken cancellationToken
    )
    {
        var lessons = await mediator.Send(new ListLessons(courseId), cancellationToken);

        return Ok(lessons);
    }

    [HttpGet("current", Name = "GetCurrentLessonProgress")]
    public async Task<ActionResult<CurrentLessonProgressResponse>> GetCurrent(
        CancellationToken cancellationToken
    )
    {
        var progress = await mediator.Send(new GetCurrentLessonProgress(), cancellationToken);

        return progress is null ? NotFound() : Ok(progress);
    }

    [HttpGet("{id:int}", Name = "GetLessonById")]
    public async Task<ActionResult<LessonDetailResponse>> GetById(
        int id,
        CancellationToken cancellationToken
    )
    {
        var lesson = await mediator.Send(new GetLessonById(id), cancellationToken);

        return lesson is null ? NotFound() : Ok(lesson);
    }

    [HttpPost("{id:int}/section-completed", Name = "OnLessonSectionCompleted")]
    public async Task<IActionResult> CompleteSection(
        int id,
        [FromQuery] StorySection section,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(new OnLessonSectionCompleted(id, section), cancellationToken);

        return NoContent();
    }
}
