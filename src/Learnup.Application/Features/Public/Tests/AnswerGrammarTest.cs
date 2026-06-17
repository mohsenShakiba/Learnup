using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record AnswerGrammarTest(int GrammarTestId, int SelectedOptionId) : IRequest<AnswerTestResponse>;

internal sealed class AnswerGrammarTestHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<AnswerGrammarTest, AnswerTestResponse>
{
    public async Task<AnswerTestResponse> Handle(AnswerGrammarTest request, CancellationToken cancellationToken)
    {
        var test = await dbContext.GrammarTests
            .Include(t => t.Options)
            .FirstOrDefaultAsync(t => t.Id == request.GrammarTestId, cancellationToken)
            ?? throw new InvalidOperationException("GrammarTest not found");

        var selectedOption = test.Options.FirstOrDefault(o => o.Id == request.SelectedOptionId)
            ?? throw new InvalidOperationException("Option not found");

        var correctOption = test.Options.First(o => o.IsCorrect);
        var isCorrect = selectedOption.IsCorrect;

        var existing = await dbContext.UserGrammarTestResults
            .FirstOrDefaultAsync(
                r => r.UserId == identityProvider.UserId && r.GrammarTestId == request.GrammarTestId,
                cancellationToken);

        if (existing is null)
        {
            dbContext.UserGrammarTestResults.Add(
                new UserGrammarTestResult(identityProvider.UserId, request.GrammarTestId, request.SelectedOptionId, isCorrect));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return new AnswerTestResponse(isCorrect, correctOption.Id);
    }
}
