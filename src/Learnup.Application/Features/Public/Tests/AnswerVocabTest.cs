using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record AnswerVocabTest(int VocabTestId, int SelectedOptionId) : IRequest<AnswerTestResponse>;

internal sealed class AnswerVocabTestHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<AnswerVocabTest, AnswerTestResponse>
{
    public async Task<AnswerTestResponse> Handle(AnswerVocabTest request, CancellationToken cancellationToken)
    {
        var test = await dbContext.VocabTests
            .Include(t => t.Options)
            .FirstOrDefaultAsync(t => t.Id == request.VocabTestId, cancellationToken)
            ?? throw new InvalidOperationException("VocabTest not found");

        var selectedOption = test.Options.FirstOrDefault(o => o.Id == request.SelectedOptionId)
            ?? throw new InvalidOperationException("Option not found");

        var correctOption = test.Options.First(o => o.IsCorrect);
        var isCorrect = selectedOption.IsCorrect;

        var existing = await dbContext.UserVocabTestResults
            .FirstOrDefaultAsync(
                r => r.UserId == identityProvider.UserId && r.VocabTestId == request.VocabTestId,
                cancellationToken);

        if (existing is null)
        {
            dbContext.UserVocabTestResults.Add(
                new UserVocabTestResult(identityProvider.UserId, request.VocabTestId, request.SelectedOptionId, isCorrect));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return new AnswerTestResponse(isCorrect, correctOption.Id);
    }
}
