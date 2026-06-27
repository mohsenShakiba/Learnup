using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record CheckTest(int LessonId, TestType Type) : IRequest;

internal sealed class CheckTestHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<CheckTest>
{
    public async Task<Unit> Handle(CheckTest request, CancellationToken cancellationToken)
    {
        var testIds = await dbContext.Tests
            .Where(t => t.LessonId == request.LessonId
                && t.Type == request.Type
                && t.Status == TestStatus.Published)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        var answeredTestIds = await dbContext.UserTestResults
            .Where(r => r.UserId == identityProvider.UserId && testIds.Contains(r.TestId))
            .Select(r => r.TestId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (answeredTestIds.Count != testIds.Count)
        {
            throw new InvalidOperationException("All test questions must be answered before checking the test");
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

        userLesson.CompleteTest(request.Type);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
