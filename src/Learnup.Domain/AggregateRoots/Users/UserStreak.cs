namespace Learnup.Domain.AggregateRoots.Users;

public class UserStreak
{
    public int Id { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public DateOnly StreakDate { get; private set; }
    public DateTime VisitedAt { get; private set; }

    public UserStreak(int userId, DateOnly streakDate, DateTime visitedAt)
    {
        UserId = userId;
        StreakDate = streakDate;
        VisitedAt = visitedAt.ToUniversalTime();
    }
}
