namespace Learnup.Application.Responses.Public.Placement;

public sealed record PlacementResultResponse(
    string PlacedLevel,
    IReadOnlyDictionary<string, int> CorrectByBand,
    int? StartingCourseId,
    IReadOnlyList<PlacementAnswerReviewResponse> Answers);

public sealed record PlacementAnswerReviewResponse(
    int QuestionId,
    int? SelectedOptionId,
    bool IsCorrect);
