using Learnup.Application.Features.Public.Lessons;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Lessons;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class LessonsController(IMediator mediator) : BasePublicController
{
    [HttpGet("course/{courseId:int}", Name = "GetLessonsByCourseId")]
    public async Task<ActionResult<IReadOnlyList<LessonResponse>>> GetByCourseId(
        int courseId,
        CancellationToken cancellationToken)
    {
        var lessons = await mediator.Send(new GetLessonsByCourseId(courseId), cancellationToken);

        return Ok(lessons);
    }

    [HttpGet("current", Name = "GetCurrentLessonProgress")]
    public async Task<ActionResult<CurrentLessonProgressResponse>> GetCurrent(CancellationToken cancellationToken)
    {
        var progress = await mediator.Send(new GetCurrentLessonProgress(), cancellationToken);

        return progress is null
            ? NotFound()
            : Ok(progress);
    }

    [HttpGet("{id:int}", Name = "GetLessonById")]
    public async Task<ActionResult<LessonDetailResponse>> GetById(
        int id,
        [FromQuery] UserLessonEntityType? lastReadEntityType,
        [FromQuery] int? lastReadEntityId,
        CancellationToken cancellationToken)
    {
        var lesson = await mediator.Send(
            new GetLessonById(id, lastReadEntityType, lastReadEntityId),
            cancellationToken);

        return lesson is null
            ? NotFound()
            : Ok(lesson);
    }

    [HttpPost("next", Name = "GoToNextLesson")]
    public async Task<ActionResult<LessonDetailResponse>> GoToNext(CancellationToken cancellationToken)
    {
        var lesson = await mediator.Send(new GoToNextLesson(), cancellationToken);

        return lesson is null
            ? NotFound()
            : Ok(lesson);
    }
}
