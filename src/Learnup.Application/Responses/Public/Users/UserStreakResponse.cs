namespace Learnup.Application.Responses.Public.Users;

public sealed record UserStreakResponse(
    int CurrentStreak,
    DateOnly? LastStreakDate,
    DateTime? LastVisitedAt,
    IReadOnlyList<UserStreakDayResponse> LastSevenDays);

public sealed record UserStreakDayResponse(
    DateOnly Date,
    bool IsCheckedIn);
