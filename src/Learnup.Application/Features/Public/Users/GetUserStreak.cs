using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Users;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Users;

public sealed record GetUserStreak : IRequest<UserStreakResponse?>;

internal sealed class GetUserStreakHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<GetUserStreak, UserStreakResponse?>
{
    public async Task<UserStreakResponse?> Handle(
        GetUserStreak request,
        CancellationToken cancellationToken)
    {
        var visitedAt = DateTime.UtcNow;
        var streakDate = DateOnly.FromDateTime(visitedAt);

        var todayStreak = await dbContext.UserStreaks
            .FirstOrDefaultAsync(
                streak => streak.UserId == identityProvider.UserId && streak.StreakDate == streakDate,
                cancellationToken);

        if (todayStreak is null)
        {
            todayStreak = new UserStreak(identityProvider.UserId, streakDate, visitedAt);
            dbContext.UserStreaks.Add(todayStreak);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var streakDates = await dbContext.UserStreaks
            .AsNoTracking()
            .Where(streak => streak.UserId == identityProvider.UserId)
            .OrderBy(streak => streak.StreakDate)
            .Select(streak => streak.StreakDate)
            .ToListAsync(cancellationToken);

        var (currentStreak, longestStreak) = CalculateStreaks(streakDates, todayStreak.StreakDate);
        var lastSevenDays = GetLastSevenDays(streakDates, todayStreak.StreakDate);

        return new UserStreakResponse(
            currentStreak,
            longestStreak,
            todayStreak.StreakDate,
            todayStreak.VisitedAt,
            lastSevenDays);
    }

    private static IReadOnlyList<UserStreakDayResponse> GetLastSevenDays(
        IReadOnlyList<DateOnly> streakDates,
        DateOnly currentDate)
    {
        var streakDateSet = streakDates.ToHashSet();

        return Enumerable
            .Range(0, 7)
            .Select(offset => currentDate.AddDays(offset - 6))
            .Select(date => new UserStreakDayResponse(date, streakDateSet.Contains(date)))
            .ToList();
    }

    private static (int CurrentStreak, int LongestStreak) CalculateStreaks(
        IReadOnlyList<DateOnly> streakDates,
        DateOnly currentDate)
    {
        var streakDateSet = streakDates.ToHashSet();
        var longestStreak = 0;
        var runningStreak = 0;
        DateOnly? previousDate = null;

        foreach (var streakDate in streakDates)
        {
            runningStreak = previousDate == streakDate.AddDays(-1)
                ? runningStreak + 1
                : 1;

            if (runningStreak > longestStreak)
            {
                longestStreak = runningStreak;
            }

            previousDate = streakDate;
        }

        var currentStreak = 0;
        for (var date = currentDate; streakDateSet.Contains(date); date = date.AddDays(-1))
        {
            currentStreak++;
        }

        return (currentStreak, longestStreak);
    }
}
