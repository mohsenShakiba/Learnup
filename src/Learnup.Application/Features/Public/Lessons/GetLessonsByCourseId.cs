using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;

public sealed record GetLessonsByCourseId(int CourseId) : IRequest<IReadOnlyList<LessonResponse>>;

internal sealed class GetLessonsByCourseIdHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetLessonsByCourseId, IReadOnlyList<LessonResponse>>
{
    public async Task<IReadOnlyList<LessonResponse>> Handle(
        GetLessonsByCourseId request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Lessons
            .AsNoTracking()
            .Where(lesson => lesson.CourseId == request.CourseId)
            .OrderBy(lesson => lesson.Order)
            .ThenBy(lesson => lesson.Id)
            .Select(lesson => lesson.ToResponse())
            .ToListAsync(cancellationToken);
    }
}
