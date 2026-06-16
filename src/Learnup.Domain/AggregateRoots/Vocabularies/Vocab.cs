using Learnup.Domain.AggregateRoots.Languages;

namespace Learnup.Domain.AggregateRoots.Vocabularies;

public class Vocab
{
    public int Id { get; private set; }
    public string Word { get; private set; }
    public string? Translation { get; private set; }
    public string? VoiceId { get; private set; }
    public string? Description { get; private set; }
    public VocabLevel Level { get; private set; }
    public VocabStatus Status { get; private set; }
    
    public string? ParentVocab { get; private set; }
    
    public int LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;

    private readonly List<VocabTranslation> _translations = [];
    public IReadOnlyList<VocabTranslation> Translations => _translations.AsReadOnly();

    public Vocab(int languageId, string word, VocabLevel level, string translation = "")
    {
        Word = word;
        Level = level;
        LanguageId = languageId;
        Translation = translation;
        Status = VocabStatus.Pending;
    }

    public void SetTranslation(string translation, string? description, string? parentVocab)
    {
        Translation = translation.Trim();
        Description = description?.Trim();
        ParentVocab = parentVocab;
    }

    public void SetVoice(string voiceId)
    {
        VoiceId = voiceId.Trim();
    }

    public void MarkAsPublished()
    {
        Status = VocabStatus.Published;
    }

    public void AddTranslation(VocabTranslation translation)
    {
        _translations.Add(translation);
    }
}
