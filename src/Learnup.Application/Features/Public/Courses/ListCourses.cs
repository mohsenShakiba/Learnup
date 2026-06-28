using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Courses;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Courses;

public sealed record ListCourses(int LanguageId) : IRequest<IReadOnlyList<CourseResponse>>;

internal sealed class ListCoursesHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<ListCourses, IReadOnlyList<CourseResponse>>
{
    public async Task<IReadOnlyList<CourseResponse>> Handle(ListCourses request, CancellationToken cancellationToken)
    {
        var courses = await dbContext.Courses
            .AsNoTracking()
            .Include(course => course.Lessons)
            .ThenInclude(lesson => lesson.Users.Where(userLesson => userLesson.UserId == identityProvider.UserId))
            .Where(course => course.LanguageId == request.LanguageId)
            .OrderBy(course => course.Order)
            .ThenBy(course => course.Id)
            .ToListAsync(cancellationToken);

        return courses.Select(course => course.ToResponse()).ToList();
    }
}
