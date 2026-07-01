using Learnup.Application.Authentication;
using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Placement;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Placement;

public sealed record GetPlacementResult : IRequest<PlacementResultResponse?>;

internal sealed class GetPlacementResultHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetPlacementResult, PlacementResultResponse?>
{
    public async Task<PlacementResultResponse?> Handle(GetPlacementResult request, CancellationToken cancellationToken)
    {
        var result = await dbContext.UserPlacementResults
            .AsNoTracking()
            .Include(entry => entry.Answers)
            .FirstOrDefaultAsync(entry => entry.UserId == identityProvider.UserId, cancellationToken);

        if (result is null)
        {
            return null;
        }

        var correctByBand = await ComputeCorrectByBandAsync(result.Answers, cancellationToken);

        var startingCourseId = await dbContext.Courses
            .AsNoTracking()
            .Where(course => course.Code == result.PlacedLevel)
            .Select(course => (int?)course.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return result.ToResponse(correctByBand, startingCourseId);
    }

    private async Task<Dictionary<string, int>> ComputeCorrectByBandAsync(
        IReadOnlyCollection<Domain.AggregateRoots.Users.UserPlacementAnswer> answers,
        CancellationToken cancellationToken)
    {
        var questionIds = answers.Select(answer => answer.PlacementQuestionId).ToList();

        var levelByQuestionId = await dbContext.PlacementQuestions
            .AsNoTracking()
            .Where(question => questionIds.Contains(question.Id))
            .ToDictionaryAsync(question => question.Id, question => question.Level, cancellationToken);

        return answers
            .Where(answer => levelByQuestionId.ContainsKey(answer.PlacementQuestionId))
            .GroupBy(answer => levelByQuestionId[answer.PlacementQuestionId])
            .ToDictionary(group => group.Key, group => group.Count(answer => answer.IsCorrect));
    }
}
