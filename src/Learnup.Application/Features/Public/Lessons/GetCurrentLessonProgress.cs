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

        var storyIds = lesson.Stories.Select(ls => ls.StoryId).ToHashSet();
        var grammarIds = lesson.Grammars.Select(lg => lg.GrammarId).ToHashSet();
        var vocabIds = lesson.Vocabs.Select(lv => lv.VocabId).ToHashSet();

        var completedStoryIds = storyIds.Count == 0
            ? []
            : await dbContext.UserStories
                .Where(us => us.UserId == identityProvider.UserId && us.CompletedAt != null && storyIds.Contains(us.StoryId))
                .Select(us => us.StoryId)
                .ToHashSetAsync(cancellationToken);

        var completedGrammarIds = grammarIds.Count == 0
            ? []
            : await dbContext.UserGrammars
                .Where(ug => ug.UserId == identityProvider.UserId && ug.CompletedAt != null && grammarIds.Contains(ug.GrammarId))
                .Select(ug => ug.GrammarId)
                .ToHashSetAsync(cancellationToken);

        var completedVocabIds = vocabIds.Count == 0
            ? []
            : await dbContext.UserVocabs
                .Where(uv => uv.UserId == identityProvider.UserId && uv.CompletedAt != null && vocabIds.Contains(uv.VocabId))
                .Select(uv => uv.VocabId)
                .ToHashSetAsync(cancellationToken);

        var isStoryCompleted = storyIds.Count == 0 || storyIds.All(completedStoryIds.Contains);
        var isGrammarCompleted = grammarIds.Count == 0 || grammarIds.All(completedGrammarIds.Contains);
        var isVocabCompleted = vocabIds.Count == 0 || vocabIds.All(completedVocabIds.Contains);

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
