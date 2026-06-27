using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Domain.AggregateRoots.LeitnerBoxes;

public class LeitnerBox
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<BoxLevel> BoxLevels => _boxLevels.AsReadOnly();
    private readonly List<BoxLevel> _boxLevels = new();

    public IReadOnlyList<LeitnerBoxItem> Items => _items.AsReadOnly();
    private readonly List<LeitnerBoxItem> _items = new();

    private LeitnerBox()
    {
    }

    public LeitnerBox(int userId)
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddLevels()
    {
        if (_boxLevels.Count > 0)
        {
            return;
        }

        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(1), Level.Level_1));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(2), Level.Level_2));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(4), Level.Level_3));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(6), Level.Level_4));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(8), Level.Level_5));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(10), Level.Level_6));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(14), Level.Level_7));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(18), Level.Level_8));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(22), Level.Level_9));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(25), Level.Level_10));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(30), Level.Level_11));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(60), Level.Level_12));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(120), Level.Level_13));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(240), Level.Level_14));
        _boxLevels.Add(new BoxLevel(TimeSpan.FromDays(365), Level.Level_15));
    }

    public void AddItem(int vocabId)
    {
        var firstLevel = _boxLevels.FirstOrDefault(level => level.Level == Level.Level_1)
            ?? throw new InvalidOperationException("Leitner box does not contain level 1.");

        var item = new LeitnerBoxItem(vocabId, firstLevel);
        item.AssignToBox(this);
        _items.Add(item);
        firstLevel.AddItem(item);
    }

    public static LeitnerBox CreateWithLevels(int userId)
    {
        var box = new LeitnerBox(userId);
        box.AddLevels();
        return box;
    }
}
