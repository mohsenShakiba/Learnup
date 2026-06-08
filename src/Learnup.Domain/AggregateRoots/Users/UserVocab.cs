using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserVocab
{
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int VocabId { get; private set; }
    public Vocab Vocab { get; private set; } = null!;

    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
}
