using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Tests;
using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record GetGrammarTests(int LessonId) : IRequest<IReadOnlyList<GrammarTestResponse>>;

internal sealed class GetGrammarTestsHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetGrammarTests, IReadOnlyList<GrammarTestResponse>>
{
    public async Task<IReadOnlyList<GrammarTestResponse>> Handle(GetGrammarTests request, CancellationToken cancellationToken)
    {
        var grammarIds = await dbContext.LessonGrammars
            .Where(lg => lg.LessonId == request.LessonId)
            .Select(lg => lg.GrammarId)
            .ToListAsync(cancellationToken);

        var tests = await dbContext.GrammarTests
            .AsNoTracking()
            .Include(t => t.Options)
            .Where(t => grammarIds.Contains(t.GrammarId) && t.Status == TestStatus.Published)
            .ToListAsync(cancellationToken);

        var testIds = tests.Select(t => t.Id).ToList();

        var userResults = await dbContext.UserGrammarTestResults
            .AsNoTracking()
            .Where(r => r.UserId == identityProvider.UserId && testIds.Contains(r.GrammarTestId))
            .ToListAsync(cancellationToken);

        return tests.Select(t =>
        {
            var result = userResults.FirstOrDefault(r => r.GrammarTestId == t.Id);
            return new GrammarTestResponse(
                t.Id,
                t.GrammarId,
                t.Question,
                t.Options.Select(o => new TestOptionResponse(o.Id, o.Text)).ToList(),
                result?.SelectedOptionId,
                result?.IsCorrect);
        }).ToList();
    }
}
