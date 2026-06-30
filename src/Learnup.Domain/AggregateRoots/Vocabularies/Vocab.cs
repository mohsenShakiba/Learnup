using Learnup.Domain.AggregateRoots.Languages;
using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Vocabularies;

public class Vocab
{
    public int Id { get; private set; }
    public string Word { get; private set; }
    public string? Translation { get; private set; }
    public string? Description { get; private set; }
    public string? VoiceId { get; private set; }

    public VocabLevel Level { get; private set; }
    public VocabStatus Status { get; private set; }

    public int LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;

    private List<VocabTypeTranslation> _typeTranslations = [];
    public IReadOnlyList<VocabTypeTranslation> TypeTranslations => _typeTranslations.AsReadOnly();

    public Vocab(int languageId, string word, VocabLevel level)
    {
        Word = word;
        Level = level;
        LanguageId = languageId;
        Status = VocabStatus.Pending;
    }

    public Vocab(int languageId, string word, string? translation, VocabLevel level, string? description, string? voiceId)
    {
        Word = word;
        Translation = translation;
        Level = level;
        LanguageId = languageId;
        Description = description;
        VoiceId = voiceId;
        Status = VocabStatus.Pending;
    }

    public void SetTranslation(string translation, string? description)
    {
        Translation = translation;
        Description = description;
    }

    public void AddType(VocabTypeTranslation vocabTypeTranslation)
    {
        _typeTranslations.Add(vocabTypeTranslation);
    }
}