namespace Learnup.Domain.AggregateRoots.Vocabularies;

public class VocabTransaction
{
    public int Id { get; private set; }

    public int VocabId { get; private set; }
    public Vocab Vocab { get; private set; } = null!;

    public string Translation { get; private set; } = null!;
    public string? Description { get; private set; }
    public VocabTransactionType Type { get; private set; }
    public string Example { get; private set; } = null!;
    public string ExampleTranslation { get; private set; } = null!;

    private VocabTransaction()
    {
    }

    public VocabTransaction(
        int vocabId,
        string translation,
        VocabTransactionType type,
        string example,
        string exampleTranslation,
        string? description = null)
    {
        VocabId = vocabId;
        Translation = translation.Trim();
        Type = type;
        Example = example.Trim();
        ExampleTranslation = exampleTranslation.Trim();
        Description = description?.Trim();
    }
}
