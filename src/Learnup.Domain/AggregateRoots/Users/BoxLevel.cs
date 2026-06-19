namespace Learnup.Domain.AggregateRoots.Users;

public class BoxLevel
{
    public int Id { get; private set; }
    public Level Level { get; private set; }

    public int LeitnerBoxId { get; private set; }
    public LeitnerBox LeitnerBox { get; private set; } = null!;
    public TimeSpan WillReviewedIn { get; private set; }

    public IReadOnlyList<LeitnerBoxItem> Items => _items.AsReadOnly();
    private readonly List<LeitnerBoxItem> _items = new();

    private BoxLevel()
    {
    }

    public void AddItem(LeitnerBoxItem item)
    {
        if (_items.Contains(item))
        {
            return;
        }

        _items.Add(item);
    }

    public void RemoveItem(LeitnerBoxItem item)
    {
        _items.Remove(item);
    }

    public BoxLevel(TimeSpan willReviewedIn, Level level)
    {
        WillReviewedIn = willReviewedIn;
        Level = level;
    }

    public void UpdateWillReviewedIn(TimeSpan willReviewedIn)
    {
        if (willReviewedIn < TimeSpan.Zero)
        {
            throw new InvalidOperationException("Review interval cannot be negative.");
        }

        WillReviewedIn = willReviewedIn;
    }
}
