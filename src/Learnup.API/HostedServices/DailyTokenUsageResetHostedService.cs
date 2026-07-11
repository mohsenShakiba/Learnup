using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.API.HostedServices;

public sealed class DailyTokenUsageResetHostedService(
    IServiceScopeFactory serviceScopeFactory,
    IConfiguration configuration,
    ILogger<DailyTokenUsageResetHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!configuration.GetValue("HostedServices:DailyTokenUsageReset:Enabled", true))
        {
            logger.LogInformation("Daily token usage reset hosted service is disabled.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(GetDelayUntilNextUtcMidnight(), stoppingToken);
                await ResetTokenUsageAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Daily token usage reset failed.");
                await Task.Delay(GetRetryDelay(), stoppingToken);
            }
        }
    }

    private async Task ResetTokenUsageAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LearnupDbContext>();

        var resetCounts = await dbContext.UserTokenUsages
            .Where(t => t.TotalTokens > 0)
            .ToListAsync(cancellationToken);

        foreach (var resetCount in resetCounts)
        {
            resetCount.Reset();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static TimeSpan GetDelayUntilNextUtcMidnight()
    {
        var now = DateTime.UtcNow;
        var nextUtcMidnight = now.Date.AddDays(1);
        return nextUtcMidnight - now;
    }

    private static TimeSpan GetRetryDelay()
    {
        return TimeSpan.FromMinutes(5);
    }
}
