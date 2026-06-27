using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
using Learnup.Domain.AggregateRoots.Tests;
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
            .Include(l => l.Vocabs).ThenInclude(lv => lv.Vocab)
            .Where(l => l.CourseId == currentLesson.CourseId
                && (l.Order > currentLesson.Order || (l.Order == currentLesson.Order && l.Id > currentLesson.Id)))
            .OrderBy(l => l.Order)
            .ThenBy(l => l.Id)
            .FirstOrDefaultAsync(cancellationToken);

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

        await dbContext.SaveChangesAsync(cancellationToken);

        var testsCount = await dbContext.Tests
            .CountAsync(t => t.LessonId == nextLesson.Id && t.Type == TestType.Vocab, cancellationToken);

        var userTests = await dbContext.UserTestResults
            .Where(t => t.UserId == identityProvider.UserId
                && t.Test.LessonId == nextLesson.Id
                && t.Test.Type == TestType.Vocab)
            .Select(t => t.IsCorrect)
            .ToListAsync(cancellationToken);

        var test = new LessonTestResponse
        {
            IsPassed = testsCount == userTests.Count,
            Score = userTests.Count == 0 || testsCount == 0 ? 0 : userTests.Count / (float)testsCount * 100
        };

        return nextLesson.ToDetailResponse(test);
    }
}
