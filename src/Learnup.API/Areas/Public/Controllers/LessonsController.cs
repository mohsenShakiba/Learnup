using Learnup.Application.Features.Public.Lessons;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Lessons;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class LessonsController(IMediator mediator) : BasePublicController
{
    [HttpGet("course/{courseId:int}")]
    public async Task<ActionResult<IReadOnlyList<LessonResponse>>> GetByCourseId(
        int courseId,
        CancellationToken cancellationToken)
    {
        var lessons = await mediator.Send(new GetLessonsByCourseId(courseId), cancellationToken);

        return Ok(lessons);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LessonDetailResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var lesson = await mediator.Send(new GetLessonById(id), cancellationToken);

        return lesson is null
            ? NotFound()
            : Ok(lesson);
    }
}
