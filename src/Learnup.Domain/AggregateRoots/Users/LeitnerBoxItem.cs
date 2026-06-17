using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Domain.AggregateRoots.Users;

public class LeitnerBoxItem
{
    public int Id { get; private set; }
    public int LeitnerBoxId { get; private set; }
    public LeitnerBox LeitnerBox { get; private set; } = null!;
    public int VocabId { get; private set; }
    public Vocab Vocab { get; private set; } = null!;
    public int BoxLevel { get; private set; }
    public DateTime AddedAt { get; private set; }

    private LeitnerBoxItem() { }

    public LeitnerBoxItem(int vocabId)
    {
        VocabId = vocabId;
        BoxLevel = 1;
        AddedAt = DateTime.UtcNow;
    }
}
