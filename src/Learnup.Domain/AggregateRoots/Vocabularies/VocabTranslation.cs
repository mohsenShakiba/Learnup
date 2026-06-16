namespace Learnup.Domain.AggregateRoots.Vocabularies;

public class VocabTranslation
{
    public int Id { get; private set; }

    public int VocabId { get; private set; }
    public Vocab Vocab { get; private set; } = null!;
    public VocabStatus Status { get; private set; }
    public VocabTranslationType Type { get; private set; }

    public string Translation { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Example { get; private set; } = null!;
    public string? ExampleTranslation { get; private set; } = null!;

    private VocabTranslation()
    {
    }

    public VocabTranslation(int vocabId, VocabTranslationType type)
    {
        VocabId = vocabId;
        Type = type;
        Status = VocabStatus.Pending;
        Translation = string.Empty;
    }
}
