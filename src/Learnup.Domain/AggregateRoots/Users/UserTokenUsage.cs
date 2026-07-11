namespace Learnup.Domain.AggregateRoots.Users;

/// <summary>
/// Tracks the number of AI tokens a user has consumed in the current daily window.
/// One row per user; totals are incremented on every AI interaction and reset daily.
/// </summary>
public class UserTokenUsage
{
    public int Id { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public long AvailableTokens { get; private set; }
    public long PromptTokens { get; private set; }
    public long CompletionTokens { get; private set; }
    public long TotalTokens { get; private set; }
    public int RequestCount { get; private set; }
    public long TotalTokensHistoryUsage { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime ResetAt { get; private set; }

    private UserTokenUsage()
    {
    }

    public UserTokenUsage(int userId)
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        Reset();
    }

    public void Add(int promptTokens, int completionTokens, DateTime usageDateUtc)
    {
        PromptTokens += promptTokens;
        CompletionTokens += completionTokens;
        TotalTokens += promptTokens + completionTokens;
        RequestCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reset()
    {
        AvailableTokens = 10_000;
        PromptTokens = 0;
        CompletionTokens = 0;
        TotalTokens = 0;
        RequestCount = 0;
        TotalTokensHistoryUsage += TotalTokens;
        ResetAt = DateTime.UtcNow;
    }
}
