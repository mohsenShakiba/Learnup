namespace Learnup.Application.Responses.Public.Users;

public sealed record UserStreakResponse(
    int CurrentStreak,
    int LongestStreak,
    DateOnly? LastStreakDate,
    DateTime? LastVisitedAt,
    IReadOnlyList<UserStreakDayResponse> LastSevenDays);

public sealed record UserStreakDayResponse(
    DateOnly Date,
    bool IsCheckedIn);
