using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Courses;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Courses;

public sealed record GetCoursesByLanguageId(int LanguageId) : IRequest<IReadOnlyList<CourseResponse>>;

internal sealed class GetCoursesByLanguageIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetCoursesByLanguageId, IReadOnlyList<CourseResponse>>
{
    public async Task<IReadOnlyList<CourseResponse>> Handle(
        GetCoursesByLanguageId request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Courses
            .AsNoTracking()
            .Where(course => course.LanguageId == request.LanguageId)
            .OrderBy(course => course.Order)
            .ThenBy(course => course.Id)
            .Select(course => new CourseResponse(
                course.Id,
                course.Code,
                course.Slug,
                course.Title,
                course.Description,
                course.Order,
                dbContext.Lessons
                    .Where(lesson => lesson.CourseId == course.Id)
                    .SelectMany(lesson => lesson.Stories)
                    .Count(),
                course.Lessons.SelectMany(l => l.Users.Where(u => u.UserId == identityProvider.UserId)).Count(),
                course.LanguageId,
                course.CoverId,
                course.Users.FirstOrDefault(u => u.UserId == identityProvider.UserId).FirstVisitedAt))
            .ToListAsync(cancellationToken);
    }
}
