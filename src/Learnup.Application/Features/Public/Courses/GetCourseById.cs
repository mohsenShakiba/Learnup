using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Courses;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Courses;

public sealed record GetCourseById(int Id) : IRequest<CourseResponse?>;

internal sealed class GetCourseByIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetCourseById, CourseResponse?>
{
    public async Task<CourseResponse?> Handle(
        GetCourseById request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Courses
            .AsNoTracking()
            .Where(course => course.Id == request.Id)
            .Select(course => new CourseResponse(
                    course.Id,
                    course.Title,
                    course.Description,
                    course.Order,
                    course.CoverId,
                    course.Lessons.Count,
                    course.Lessons.SelectMany(l => l.Users.Where(u => u.UserId == identityProvider.UserId)).Count(),
                    course.LanguageId
                )
            )
            .FirstOrDefaultAsync(cancellationToken);
    }
}