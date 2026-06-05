using Learnup.Domain.AggregateRoots.Languages;

namespace Learnup.Domain.AggregateRoots.Vocabularies;

public class Vocab
{
    public int Id { get; private set; }
    public string Word { get; private set; }
    public string? Translation { get; private set; }
    public string? VoiceId { get; private set; }
    public string? Description { get; private set; }
    public VocalLevel Level { get; private set; }
    
    public int? ParentVocabId { get; private set; }
    public Vocab? ParentVocab { get; private set; }
    
    public int LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;

    public Vocab(int languageId, string word, VocalLevel level, string translation = "")
    {
        Word = word;
        Level = level;
        LanguageId = languageId;
        Translation = translation;
    }

    public void SetTranslation(string translation, string? description, int? parentId)
    {
        Translation = translation.Trim();
        Description = description?.Trim();
        ParentVocabId = parentId;
    }
}
