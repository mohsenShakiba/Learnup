using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record ResetVocabTestResult(int LessonId) : IRequest;

internal sealed class ResetVocabTestResultHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<ResetVocabTestResult>
{
    public async Task<Unit> Handle(ResetVocabTestResult request, CancellationToken cancellationToken)
    {
        var lessonVocabIds = await dbContext.LessonVocabs
            .Where(l => l.LessonId == request.LessonId)
            .Select(l => l.VocabId)
            .ToListAsync(cancellationToken);
        
        var userTests = await dbContext.UserVocabTestResults
            .Where(t => t.UserId == identityProvider.UserId && lessonVocabIds.Contains(t.VocabTest.VocabId))
            .ToListAsync(cancellationToken);

        dbContext.UserVocabTestResults.RemoveRange(userTests);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
