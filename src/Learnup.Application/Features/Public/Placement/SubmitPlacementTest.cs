using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Placement;

public sealed record SubmitPlacementTest(IReadOnlyList<PlacementSubmissionAnswer> Answers) : IRequest;

public sealed record PlacementSubmissionAnswer(int QuestionId, int SelectedOptionId);

internal sealed class SubmitPlacementTestHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<SubmitPlacementTest>
{
    public async Task<Unit> Handle(SubmitPlacementTest request, CancellationToken cancellationToken)
    {
        var placementTest = await dbContext.PlacementTests
            .Include(test => test.Questions)
            .ThenInclude(question => question.Options)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("Placement test has not been configured.");

        var selectedOptionByQuestion = request.Answers
            .GroupBy(answer => answer.QuestionId)
            .ToDictionary(group => group.Key, group => group.Last().SelectedOptionId);

        var correctByBand = new Dictionary<string, int>();
        var answers = new List<UserPlacementAnswer>();

        foreach (var question in placementTest.Questions.OrderBy(question => question.Number))
        {
            correctByBand.TryAdd(question.Level, 0);

            var hasAnswer = selectedOptionByQuestion.TryGetValue(question.Id, out var selectedOptionId);
            var selectedOption = question.Options.FirstOrDefault(option => option.Id == selectedOptionId);
            var isCorrect = selectedOption is { IsCorrect: true };

            if (isCorrect)
            {
                correctByBand[question.Level]++;
            }

            answers.Add(new UserPlacementAnswer(
                question.Id,
                hasAnswer ? selectedOptionId : null,
                isCorrect));
        }

        var placedLevel = PlacementScorer.Score(correctByBand);

        var existing = await dbContext.UserPlacementResults
            .Include(result => result.Answers)
            .FirstOrDefaultAsync(result => result.UserId == identityProvider.UserId, cancellationToken);

        if (existing is null)
        {
            dbContext.UserPlacementResults.Add(new UserPlacementResult(identityProvider.UserId, placedLevel, answers));
        }
        else
        {
            existing.Update(placedLevel, answers);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
