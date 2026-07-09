namespace Learnup.Domain.AggregateRoots.Stories;

[Flags]
public enum StoryStatus
{
    Pending = 0,
    Translated = 1,
    Voiced = 2,
}