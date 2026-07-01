namespace Learnup.Domain.AggregateRoots.Placement;

public class PlacementOption
{
    public int Id { get; private set; }
    public int PlacementQuestionId { get; private set; }
    public string Text { get; private set; } = null!;
    public bool IsCorrect { get; private set; }

    private PlacementOption()
    {
    }

    public PlacementOption(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}
