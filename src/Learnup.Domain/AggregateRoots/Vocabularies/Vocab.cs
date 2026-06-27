using Learnup.Domain.AggregateRoots.Languages;
using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Vocabularies;

public class Vocab
{
    public int Id { get; private set; }
    public string Word { get; private set; }
    public string? Translation { get; private set; }
    public string? Description { get; private set; }
    public string? Example { get; private set; }
    public string? ExampleTranslation { get; private set; }
    public string? VoiceId { get; private set; }
    public VocabLevel Level { get; private set; }
    public VocabStatus Status { get; private set; }
    public VocabType Type { get; private set; }

    public int LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;

    public Vocab(int languageId, string word,  VocabType type, VocabLevel level)
    {
        Word = word;
        Level = level;
        LanguageId = languageId;
        Type = type;
        Status = VocabStatus.Pending;
    }

    public Vocab(
        int languageId,
        string word,
        string? translation,
        VocabType type,
        VocabLevel level,
        string? description,
        string? example,
        string? exampleTranslation,
        string? voiceId)
    {
        Word = word;
        Translation = translation;
        Level = level;
        LanguageId = languageId;
        Type = type;
        Description = description;
        Example = example;
        ExampleTranslation = exampleTranslation;
        VoiceId = voiceId;
        Status = VocabStatus.Pending;
    }

    public void UpdateImportDetails(
        string? translation,
        VocabType type,
        string? description,
        string? example,
        string? exampleTranslation,
        string? voiceId)
    {
        Translation = translation;
        Type = type;
        Description = description;
        Example = example;
        ExampleTranslation = exampleTranslation;
        VoiceId = voiceId;
    }
}
