namespace Learnup.Domain.AggregateRoots.AudioBooks;

[Flags]
public enum AudioBookStatus
{
    Pending = 0,
    Translated = 1,
    Voiced = 2,
}