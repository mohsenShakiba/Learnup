using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record ResetGrammarTestResult(int LessonId) : IRequest;

internal sealed class ResetGrammarTestResultHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<ResetGrammarTestResult>
{
    public async Task<Unit> Handle(ResetGrammarTestResult request, CancellationToken cancellationToken)
    {
        var lessonGrammarIds = await dbContext.LessonGrammars
            .Where(l => l.LessonId == request.LessonId)
            .Select(l => l.GrammarId)
            .ToListAsync(cancellationToken);

        var userTests = await dbContext.UserGrammarTestResults
            .Where(t => t.UserId == identityProvider.UserId && lessonGrammarIds.Contains(t.GrammarTest.GrammarId))
            .ToListAsync(cancellationToken);

        dbContext.UserGrammarTestResults.RemoveRange(userTests);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
