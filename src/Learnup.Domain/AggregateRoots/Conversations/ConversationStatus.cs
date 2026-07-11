namespace Learnup.Domain.AggregateRoots.Conversations;

[Flags]
public enum ConversationStatus
{
    Pending = 0,
    Translated = 1,
    Voiced = 2,
}