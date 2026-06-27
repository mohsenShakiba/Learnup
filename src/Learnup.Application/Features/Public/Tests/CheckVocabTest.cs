using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record CheckVocabTest(int LessonId) : IRequest;

internal sealed class CheckVocabTestHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<CheckVocabTest>
{
    public async Task<Unit> Handle(CheckVocabTest request, CancellationToken cancellationToken)
    {
        // get vocab ids for the lesson
        var vocabIds = await dbContext.LessonVocabs
            .Where(lv => lv.LessonId == request.LessonId)
            .Select(lv => lv.VocabId)
            .ToListAsync(cancellationToken);

        var testIds = await dbContext.VocabTests
            .Where(t => vocabIds.Contains(t.VocabId) && t.Status == TestStatus.Published)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        var answeredTestIds = await dbContext.UserVocabTestResults
            .Where(r => r.UserId == identityProvider.UserId && testIds.Contains(r.VocabTestId))
            .Select(r => r.VocabTestId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (answeredTestIds.Count != testIds.Count)
        {
            return Unit.Value;
        }

        var userVocabs = await dbContext.UserVocabs
            .Where(uv => uv.UserId == identityProvider.UserId && vocabIds.Contains(uv.VocabId))
            .ToListAsync(cancellationToken);

        foreach (var vocabId in vocabIds)
        {
            var userVocab = userVocabs.FirstOrDefault(uv => uv.VocabId == vocabId);
            if (userVocab is null)
            {
                userVocab = new UserVocab(identityProvider.UserId, vocabId);
                dbContext.UserVocabs.Add(userVocab);
            }

            userVocab.Complete();
        }

        var userLesson = await dbContext.UserLessons
            .FirstOrDefaultAsync(
                ul => ul.UserId == identityProvider.UserId && ul.LessonId == request.LessonId,
                cancellationToken);

        if (userLesson is null)
        {
            userLesson = new UserLesson(identityProvider.UserId, request.LessonId);
            dbContext.UserLessons.Add(userLesson);
        }
        else
        {
            userLesson.Touch();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
