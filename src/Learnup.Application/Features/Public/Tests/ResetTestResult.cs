using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record ResetTestResult(int LessonId, TestType Type) : IRequest;

internal sealed class ResetTestResultHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<ResetTestResult>
{
    public async Task<Unit> Handle(ResetTestResult request, CancellationToken cancellationToken)
    {
        var userTests = await dbContext.UserTestResults
            .Where(t => t.UserId == identityProvider.UserId
                && t.Test.LessonId == request.LessonId
                && t.Test.Type == request.Type)
            .ToListAsync(cancellationToken);

        dbContext.UserTestResults.RemoveRange(userTests);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
