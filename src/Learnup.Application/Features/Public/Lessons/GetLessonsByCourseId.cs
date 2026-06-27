using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;

public sealed record GetLessonsByCourseId(int CourseId) : IRequest<IReadOnlyList<LessonResponse>>;

internal sealed class GetLessonsByCourseIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetLessonsByCourseId, IReadOnlyList<LessonResponse>>
{
    public async Task<IReadOnlyList<LessonResponse>> Handle(
        GetLessonsByCourseId request,
        CancellationToken cancellationToken)
    {
        var lessons = await dbContext.Lessons
            .AsNoTracking()
            .Include(l => l.Users.Where(u => u.UserId == identityProvider.UserId))
            .Where(l => l.CourseId == request.CourseId)
            .OrderBy(l => l.Order)
            .ThenBy(l => l.Id)
            .ToListAsync(cancellationToken);

        return lessons
            .Select(l => l.ToResponse())
            .ToList();
    }
}
