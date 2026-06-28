using Learnup.Application.Features.Public.Courses;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Courses;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class CoursesController(IMediator mediator) : BasePublicController
{
    [HttpGet("language/{languageId:int}", Name = "GetCoursesByLanguageId")]
    public async Task<ActionResult<IReadOnlyList<CourseResponse>>> GetByLanguageId(
        int languageId,
        CancellationToken cancellationToken)
    {
        var courses = await mediator.Send(new ListCourses(languageId), cancellationToken);

        return Ok(courses);
    }

    [HttpGet("{id:int}", Name = "GetCourseById")]
    public async Task<ActionResult<CourseResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var course = await mediator.Send(new GetCourseById(id), cancellationToken);

        return course is null
            ? NotFound()
            : Ok(course);
    }
}
