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
            .AsSplitQuery()
            .Include(ul => ul.Lesson)
            .ThenInclude(l => l.Course)
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

        return new CurrentLessonProgressResponse(
            lesson.Id,
            lesson.Title,
lesson.Course.Code,
lesson.Course.Slug,
            lesson.Order,
            lesson.CourseId,
            currentUserLesson.IsConversationCompleted,
            currentUserLesson.IsGrammarCompleted,
            currentUserLesson.IsVocabCompleted,
            currentUserLesson.IsTestCompleted,
            nextLessonId);
    }
}
