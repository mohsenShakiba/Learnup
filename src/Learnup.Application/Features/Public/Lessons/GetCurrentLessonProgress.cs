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
    public async Task<CurrentLessonProgressResponse?> Handle(
        GetCurrentLessonProgress request,
        CancellationToken cancellationToken)
    {
        var currentUserLesson = await dbContext.UserLessons
            .AsNoTracking()
            .Include(ul => ul.Lesson).ThenInclude(l => l.Stories)
            .Include(ul => ul.Lesson).ThenInclude(l => l.Grammars)
            .Include(ul => ul.Lesson).ThenInclude(l => l.Vocabs)
            .Where(ul => ul.UserId == identityProvider.UserId && ul.CompletedAt == null)
            .OrderByDescending(ul => ul.LastVisitedAt)
            .ThenByDescending(ul => ul.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (currentUserLesson is null)
            return null;

        var lesson = currentUserLesson.Lesson;

        var isStoryCompleted = lesson.Stories.Count == 0 || currentUserLesson.IsStoryCompleted;
        var isGrammarCompleted = lesson.Grammars.Count == 0 || currentUserLesson.IsGrammarCompleted;
        var isVocabCompleted = lesson.Vocabs.Count == 0 || currentUserLesson.IsVocabCompleted;

        int? nextLessonId = null;
        if (isStoryCompleted && isGrammarCompleted && isVocabCompleted)
        {
            nextLessonId = await dbContext.Lessons
                .Where(l => l.CourseId == lesson.CourseId
                    && (l.Order > lesson.Order || (l.Order == lesson.Order && l.Id > lesson.Id)))
                .OrderBy(l => l.Order)
                .ThenBy(l => l.Id)
                .Select(l => (int?)l.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new CurrentLessonProgressResponse(
            lesson.Id,
            lesson.Title,
            lesson.Order,
            lesson.CourseId,
            isStoryCompleted,
            isGrammarCompleted,
            isVocabCompleted,
            nextLessonId);
    }
}
