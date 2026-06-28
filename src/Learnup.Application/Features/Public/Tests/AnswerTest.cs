using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record AnswerTest(int TestId, int SelectedOptionId) : IRequest<AnswerTestResponse>;

internal sealed class AnswerTestHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<AnswerTest, AnswerTestResponse>
{
    public async Task<AnswerTestResponse> Handle(AnswerTest request, CancellationToken cancellationToken)
    {
        var test = await dbContext.Tests
                       .Include(t => t.Options)
                       .FirstOrDefaultAsync(t => t.Id == request.TestId, cancellationToken)
                   ?? throw new InvalidOperationException("Test not found");

        var selectedOption = test.Options
                                 .FirstOrDefault(o => o.Id == request.SelectedOptionId)
                             ?? throw new InvalidOperationException("Option not found");

        var correctOption = test.Options.First(o => o.IsCorrect);
        var isCorrect = selectedOption.IsCorrect;

        var existing = await dbContext.UserTestResults
            .FirstOrDefaultAsync(
                r => r.UserId == identityProvider.UserId && r.TestId == request.TestId,
                cancellationToken);

        if (existing is null)
        {
            var userTestResult = new UserTestResult(identityProvider.UserId, request.TestId, request.SelectedOptionId, isCorrect);
            dbContext.UserTestResults.Add(userTestResult);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            existing.UpdateSelectedOption(request.SelectedOptionId, isCorrect);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return new AnswerTestResponse(isCorrect, correctOption.Id);
    }
}