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
    public string? ParentVocab { get; private set; }
    public string? ParentVocabDescription { get; private set; }

    public VocabLevel Level { get; private set; }
    public VocabStatus Status { get; private set; }
    public VocabSource Source { get; private set; }

    public int LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;

    private List<VocabSense> _senses = [];
    public IReadOnlyList<VocabSense> Senses => _senses.AsReadOnly();

    public Vocab(int languageId, string word, VocabLevel level, VocabSource source = VocabSource.System)
    {
        Word = word;
        Level = level;
        LanguageId = languageId;
        Status = VocabStatus.Pending;
        Source = source;
    }

    public Vocab(int languageId, string word, string? translation, VocabLevel level, string? description, string? voiceId, VocabSource source = VocabSource.System)
    {
        Word = word;
        Translation = translation;
        Level = level;
        LanguageId = languageId;
        Description = description;
        VoiceId = voiceId;
        Status = VocabStatus.Pending;
        Source = source;

        if (!string.IsNullOrWhiteSpace(translation))
        {
            MarkAsTranslated();
        }

        if (!string.IsNullOrWhiteSpace(voiceId))
        {
            MarkAsVoiced();
        }
    }

    public void SetTranslation(string translation, string? description)
    {
        Translation = translation;
        Description = description;

        if (!string.IsNullOrWhiteSpace(translation))
        {
            MarkAsTranslated();
        }
    }

    public void SetParentVocab(string? parentVocab, string? parentVocabDescription)
    {
        ParentVocab = string.IsNullOrWhiteSpace(parentVocab)
            ? null
            : parentVocab.Trim();

        ParentVocabDescription = string.IsNullOrWhiteSpace(parentVocabDescription)
            ? null
            : parentVocabDescription.Trim();
    }

    public bool IsTranslated => Status.HasFlag(VocabStatus.Translated);

    public bool IsVoiced => Status.HasFlag(VocabStatus.Voiced);

    public void MarkAsTranslated()
    {
        Status |= VocabStatus.Translated;
    }

    public void MarkAsVoiced()
    {
        Status |= VocabStatus.Voiced;
    }

    public void SetVoice(string voiceId)
    {
        VoiceId = voiceId;
        MarkAsVoiced();
    }

    public void AddType(VocabSense vocabSense)
    {
        _senses.Add(vocabSense);
    }
}
