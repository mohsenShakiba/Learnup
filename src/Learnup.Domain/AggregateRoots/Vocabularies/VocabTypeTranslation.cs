namespace Learnup.Domain.AggregateRoots.Vocabularies;

public class VocabTypeTranslation
{
    public int Id { get; private set; }
    
    public string? Translation { get; private set; }
    public string? Description { get; private set; }
    public string? Example { get; private set; }
    public string? ExampleTranslation { get; private set; }
    public VocabType Type { get; private set; }

    public int VocabId { get; private set; }
    public Vocab Vocab { get; private set; }

    public VocabTypeTranslation(int vocabId, string translation, string? description, string example, string exampleTranslation, VocabType type)
    {
        Translation = translation;
        Description = description;
        Example = example;
        ExampleTranslation = exampleTranslation;
        Type = type;
        VocabId = vocabId;
    }
}