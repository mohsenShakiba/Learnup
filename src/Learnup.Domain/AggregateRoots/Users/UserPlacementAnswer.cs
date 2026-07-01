namespace Learnup.Domain.AggregateRoots.Users;

/// <summary>
/// A single question the learner answered during a placement attempt. Stores only the link to the
/// question, the picked option (null if unanswered) and whether it was correct — the question text
/// and options are served separately by the placement test endpoint.
/// </summary>
public class UserPlacementAnswer
{
    public int Id { get; private set; }
    public int UserPlacementResultId { get; private set; }
    public int PlacementQuestionId { get; private set; }
    public int? SelectedOptionId { get; private set; }
    public bool IsCorrect { get; private set; }

    private UserPlacementAnswer()
    {
    }

    public UserPlacementAnswer(int placementQuestionId, int? selectedOptionId, bool isCorrect)
    {
        PlacementQuestionId = placementQuestionId;
        SelectedOptionId = selectedOptionId;
        IsCorrect = isCorrect;
    }
}
