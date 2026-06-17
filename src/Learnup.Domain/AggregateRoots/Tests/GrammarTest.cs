namespace Learnup.Domain.AggregateRoots.Tests;

public class GrammarTest
{
    private readonly List<GrammarTestOption> _options = new();

    public int Id { get; private set; }
    public int GrammarId { get; private set; }
    public string Question { get; private set; } = null!;
    public TestStatus Status { get; private set; }

    public IReadOnlyList<GrammarTestOption> Options => _options.AsReadOnly();

    public GrammarTest(int grammarId)
    {
        GrammarId = grammarId;
        Status = TestStatus.Pending;
    }

    public void Publish(string question, IEnumerable<GrammarTestOption> options)
    {
        Question = question;
        _options.Clear();
        _options.AddRange(options);
        Status = TestStatus.Published;
    }
}
