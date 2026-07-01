namespace Learnup.API.Requests;

public sealed record SubmitPlacementRequest(IReadOnlyList<PlacementAnswerDto> Answers);

public sealed record PlacementAnswerDto(int QuestionId, int SelectedOptionId);
