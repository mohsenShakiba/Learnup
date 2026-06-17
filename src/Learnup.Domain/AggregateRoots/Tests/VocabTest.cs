using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Domain.AggregateRoots.Tests;

public class VocabTest
{
    private readonly List<VocabTestOption> _options = new();

    public int Id { get; private set; }
    public TestStatus Status { get; private set; }
    
    public int VocabId { get; private set; }
    public Vocab Vocab { get; private set; } = null!;
    
    public string Question { get; private set; } = null!;

    public IReadOnlyList<VocabTestOption> Options => _options.AsReadOnly();

    public VocabTest(int vocabId)
    {
        VocabId = vocabId;
        Status = TestStatus.Pending;
    }

    public void Publish(string question, IEnumerable<VocabTestOption> options)
    {
        Question = question;
        _options.Clear();
        _options.AddRange(options);
        Status = TestStatus.Published;
    }
}
