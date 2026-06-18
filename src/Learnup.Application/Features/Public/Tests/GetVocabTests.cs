using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Tests;
using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record GetVocabTests(int LessonId) : IRequest<IReadOnlyList<VocabTestResponse>>;

internal sealed class GetVocabTestsHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetVocabTests, IReadOnlyList<VocabTestResponse>>
{
    public async Task<IReadOnlyList<VocabTestResponse>> Handle(GetVocabTests request, CancellationToken cancellationToken)
    {
        var vocabIds = await dbContext.LessonVocabs
            .Where(lv => lv.LessonId == request.LessonId)
            .Select(lv => lv.VocabId)
            .ToListAsync(cancellationToken);

        var tests = await dbContext.VocabTests
            .AsNoTracking()
            .Include(t => t.Options)
            .Where(t => vocabIds.Contains(t.VocabId) && t.Status == TestStatus.Published)
            .ToListAsync(cancellationToken);

        var testIds = tests.Select(t => t.Id).ToList();

        var userResults = await dbContext.UserVocabTestResults
            .AsNoTracking()
            .Where(r => r.UserId == identityProvider.UserId && testIds.Contains(r.VocabTestId))
            .ToListAsync(cancellationToken);

        return tests.Select(t =>
        {
            var result = userResults.FirstOrDefault(r => r.VocabTestId == t.Id);
            return new VocabTestResponse(
                t.Id,
                t.VocabId,
                t.Question,
                t.Type,
                t.Options.Select(o => new TestOptionResponse(o.Id, o.Text)).ToList(),
                result?.SelectedOptionId,
                result?.IsCorrect);
        }).ToList();
    }
}
