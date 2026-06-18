using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record ResetGrammarTestResult(int GrammarTestId) : IRequest;

internal sealed class ResetGrammarTestResultHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<ResetGrammarTestResult>
{
    public async Task<Unit> Handle(ResetGrammarTestResult request, CancellationToken cancellationToken)
    {
        var result = await dbContext.UserGrammarTestResults
            .FirstOrDefaultAsync(
                r => r.UserId == identityProvider.UserId && r.GrammarTestId == request.GrammarTestId,
                cancellationToken);

        if (result is null)
        {
            return Unit.Value;
        }

        dbContext.UserGrammarTestResults.Remove(result);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
