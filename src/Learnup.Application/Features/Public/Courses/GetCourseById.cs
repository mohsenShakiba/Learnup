using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Courses;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Courses;

public sealed record GetCourseById(int Id) : IRequest<CourseResponse?>;

internal sealed class GetCourseByIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetCourseById, CourseResponse?>
{
    public async Task<CourseResponse?> Handle(GetCourseById request, CancellationToken cancellationToken)
    {
        var course = await dbContext.Courses
            .AsNoTracking()
            .Include(course => course.Lessons)
            .ThenInclude(lesson => lesson.Users.Where(userLesson => userLesson.UserId == identityProvider.UserId))
            .Where(course => course.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return course?.ToResponse();
    }
}
