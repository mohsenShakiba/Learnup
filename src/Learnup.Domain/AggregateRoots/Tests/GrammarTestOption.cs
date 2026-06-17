namespace Learnup.Domain.AggregateRoots.Tests;

public class GrammarTestOption
{
    public int Id { get; private set; }
    public int GrammarTestId { get; private set; }
    public string Text { get; private set; } = null!;
    public bool IsCorrect { get; private set; }

    private GrammarTestOption() { }

    public GrammarTestOption(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}
