namespace Learnup.Domain.AggregateRoots.Vocabularies;

[Flags]
public enum VocabStatus
{
    Pending = 0,
    Translated = 1,
    Voiced = 2,
}
