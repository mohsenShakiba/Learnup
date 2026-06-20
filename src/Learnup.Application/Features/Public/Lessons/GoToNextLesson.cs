using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;

public sealed record GoToNextLesson : IRequest<LessonDetailResponse?>;

internal sealed class GoToNextLessonHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GoToNextLesson, LessonDetailResponse?>
{
    public async Task<LessonDetailResponse?> Handle(
        GoToNextLesson request,
        CancellationToken cancellationToken)
    {
        var currentUserLesson = await dbContext.UserLessons
            .Include(ul => ul.Lesson)
            .Where(ul => ul.UserId == identityProvider.UserId && ul.CompletedAt == null)
            .OrderByDescending(ul => ul.LastVisitedAt)
            .ThenByDescending(ul => ul.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (currentUserLesson is null)
        {
            return null;
        }

        var currentLesson = currentUserLesson.Lesson;
        var nextLesson = await dbContext.Lessons
            .Include(l => l.Stories).ThenInclude(ls => ls.Story).ThenInclude(s => s.Items)
            .Include(l => l.Grammars).ThenInclude(lg => lg.Grammar)
            .Include(l => l.Vocabs).ThenInclude(lv => lv.Vocab).ThenInclude(v => v.Tests)
            .Where(l => l.CourseId == currentLesson.CourseId
                && (l.Order > currentLesson.Order || (l.Order == currentLesson.Order && l.Id > currentLesson.Id)))
            .OrderBy(l => l.Order)
            .ThenBy(l => l.Id)
            .FirstOrDefaultAsync(cancellationToken);

        currentUserLesson.Complete();

        if (nextLesson is null)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return null;
        }

        var nextUserLesson = await dbContext.UserLessons
            .FirstOrDefaultAsync(
                ul => ul.UserId == identityProvider.UserId && ul.LessonId == nextLesson.Id,
                cancellationToken);

        if (nextUserLesson is null)
        {
            nextUserLesson = new UserLesson(identityProvider.UserId, nextLesson.Id);
            dbContext.UserLessons.Add(nextUserLesson);
        }
        else
        {
            nextUserLesson.Touch();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var vocabIds = nextLesson.Vocabs.Select(lv => lv.VocabId).ToList();
        var vocabTestsCount = nextLesson.Vocabs.SelectMany(lv => lv.Vocab.Tests).Count();

        var userVocabTests = await dbContext.UserVocabTestResults
            .Where(t => t.UserId == identityProvider.UserId && vocabIds.Contains(t.VocabTest.Vocab.Id))
            .Select(t => t.IsCorrect)
            .ToListAsync(cancellationToken);

        var vocabTest = new LessonVocabTestResponse
        {
            IsPassed = vocabTestsCount == userVocabTests.Count,
            Score = userVocabTests.Count == 0 ? 0 : userVocabTests.Count / (float)vocabTestsCount * 100
        };

        return nextLesson.ToDetailResponse(vocabTest);
    }
}
