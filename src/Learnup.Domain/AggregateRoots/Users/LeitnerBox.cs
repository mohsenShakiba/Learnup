namespace Learnup.Domain.AggregateRoots.Users;

public class LeitnerBox
{
    private readonly List<LeitnerBoxItem> _items = new();

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<LeitnerBoxItem> Items => _items.AsReadOnly();

    private LeitnerBox() { }

    public LeitnerBox(int userId)
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(int vocabId)
    {
        if (_items.Any(i => i.VocabId == vocabId))
            return;

        _items.Add(new LeitnerBoxItem(vocabId));
    }
}
