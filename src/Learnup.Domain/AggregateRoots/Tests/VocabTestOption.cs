namespace Learnup.Domain.AggregateRoots.Tests;

public class VocabTestOption
{
    public int Id { get; private set; }
    public int VocabTestId { get; private set; }
    public string Text { get; private set; } = null!;
    public bool IsCorrect { get; private set; }

    private VocabTestOption() { }

    public VocabTestOption(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}
