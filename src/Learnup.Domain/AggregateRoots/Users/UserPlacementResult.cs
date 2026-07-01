namespace Learnup.Domain.AggregateRoots.Users;

public class UserPlacementResult
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string PlacedLevel { get; private set; } = null!;
    public DateTime TakenAt { get; private set; }

    private readonly List<UserPlacementAnswer> _answers = [];
    public IReadOnlyList<UserPlacementAnswer> Answers => _answers.AsReadOnly();

    private UserPlacementResult()
    {
    }

    public UserPlacementResult(
        int userId,
        string placedLevel,
        IEnumerable<UserPlacementAnswer> answers)
    {
        UserId = userId;
        PlacedLevel = placedLevel;
        TakenAt = DateTime.UtcNow;
        _answers.AddRange(answers);
    }

    public void Update(string placedLevel, IEnumerable<UserPlacementAnswer> answers)
    {
        PlacedLevel = placedLevel;
        TakenAt = DateTime.UtcNow;
        _answers.Clear();
        _answers.AddRange(answers);
    }
}
