using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Courses;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Courses;

public sealed record GetCourseById(int Id) : IRequest<CourseDetailResponse?>;

internal sealed class GetCourseByIdHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetCourseById, CourseDetailResponse?>
{
    public async Task<CourseDetailResponse?> Handle(
        GetCourseById request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Courses
            .AsNoTracking()
            .Where(course => course.Id == request.Id)
            .Select(course => new CourseDetailResponse(
                course.Id,
                course.Title,
                course.Order,
                course.LanguageId,
                dbContext.Lessons
                    .AsNoTracking()
                    .Where(lesson => lesson.CourseId == course.Id)
                    .OrderBy(lesson => lesson.Order)
                    .ThenBy(lesson => lesson.Id)
                    .Select(lesson => new CourseLessonResponse(
                        lesson.Id,
                        lesson.Title,
                        lesson.Order,
                        lesson.Status))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
