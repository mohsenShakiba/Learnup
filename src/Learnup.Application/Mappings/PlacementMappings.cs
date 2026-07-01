using Learnup.Application.Responses.Public.Placement;
using Learnup.Domain.AggregateRoots.Placement;
using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Application.Mappings;

public static class PlacementMappings
{
    public static PlacementTestResponse ToResponse(this PlacementTest placementTest)
    {
        return new PlacementTestResponse(
            placementTest.Id,
            placementTest.Title,
            placementTest.Description,
            placementTest.Instructions,
            placementTest.Questions
                .OrderBy(question => question.Number)
                .Select(question => question.ToResponse())
                .ToArray());
    }

    public static PlacementQuestionResponse ToResponse(this PlacementQuestion question)
    {
        return new PlacementQuestionResponse(
            question.Id,
            question.Number,
            question.Level,
            question.Skill,
            question.Prompt,
            question.Options
                .Select(option => option.ToResponse())
                .ToArray());
    }

    // No IsCorrect flag — the correct answer is never projected into the public test.
    public static PlacementOptionResponse ToResponse(this PlacementOption option)
    {
        return new PlacementOptionResponse(option.Id, option.Text);
    }

    public static PlacementResultResponse ToResponse(
        this UserPlacementResult result,
        IReadOnlyDictionary<string, int> correctByBand,
        int? startingCourseId)
    {
        return new PlacementResultResponse(
            result.PlacedLevel,
            correctByBand,
            startingCourseId,
            result.Answers
                .Select(answer => answer.ToResponse())
                .ToArray());
    }

    public static PlacementAnswerReviewResponse ToResponse(this UserPlacementAnswer answer)
    {
        return new PlacementAnswerReviewResponse(
            answer.PlacementQuestionId,
            answer.SelectedOptionId,
            answer.IsCorrect);
    }
}
