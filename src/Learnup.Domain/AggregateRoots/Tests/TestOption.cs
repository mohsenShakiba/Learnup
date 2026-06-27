namespace Learnup.Domain.AggregateRoots.Tests;

public class TestOption
{
    public int Id { get; private set; }
    public int TestId { get; private set; }
    public string Text { get; private set; } = null!;
    public bool IsCorrect { get; private set; }

    private TestOption()
    {
    }

    public TestOption(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}
