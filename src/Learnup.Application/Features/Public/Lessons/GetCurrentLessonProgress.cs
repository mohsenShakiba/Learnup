using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;

public sealed record GetCurrentLessonProgress : IRequest<CurrentLessonProgressResponse?>;

internal sealed class GetCurrentLessonProgressHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetCurrentLessonProgress, CurrentLessonProgressResponse?>
{
    public async Task<CurrentLessonProgressResponse?> Handle(GetCurrentLessonProgress request, CancellationToken cancellationToken)
    {
        var currentUserLesson = await dbContext.UserLessons
            .AsNoTracking()
            .Include(ul => ul.Lesson)
            .Where(ul => ul.UserId == identityProvider.UserId && ul.CompletedAt == null)
            .OrderByDescending(ul => ul.LastVisitedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (currentUserLesson is null)
            return null;

        var lesson = currentUserLesson.Lesson;
        
        int? nextLessonId = null;
        if (currentUserLesson.IsCompleted())
        {
            nextLessonId = await dbContext.Lessons
                .Where(l => l.CourseId == lesson.CourseId && l.Order > lesson.Order)
                .OrderBy(l => l.Order)
                .Select(l => l.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (nextLessonId is null)
        {
            return null;
        }

        return new CurrentLessonProgressResponse(
            lesson.Id,
            lesson.Title,
            lesson.Order,
            lesson.CourseId,
            currentUserLesson.IsStoryCompleted,
            currentUserLesson.IsGrammarCompleted,
            currentUserLesson.IsVocabCompleted,
            nextLessonId);
    }
}
