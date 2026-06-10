using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Courses;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Courses;

public sealed record GetCourseById(int Id) : IRequest<CourseResponse?>;

internal sealed class GetCourseByIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetCourseById, CourseResponse?>
{
    public async Task<CourseResponse?> Handle(GetCourseById request, CancellationToken cancellationToken)
    {
        
        var existingUserCourse = dbContext.UserCourses
            .FirstOrDefault(uc => uc.UserId == identityProvider.UserId && uc.CourseId == request.Id);

        if (existingUserCourse is null)
        {
            existingUserCourse = new UserCourse(identityProvider.UserId, request.Id);
            
            dbContext.UserCourses.Add(existingUserCourse);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        
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
                    course.LanguageId,
                    course.Users.FirstOrDefault(u => u.UserId == identityProvider.UserId).FirstVisitedAt
                )
            )
            .FirstOrDefaultAsync(cancellationToken);
    }
}