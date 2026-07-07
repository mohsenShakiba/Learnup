namespace Learnup.Domain.AggregateRoots.Users;

/// <summary>
/// Tracks the cumulative number of AI tokens a user has consumed.
/// One row per user; totals are incremented on every AI interaction.
/// </summary>
public class UserTokenUsage
{
    public int Id { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public long PromptTokens { get; private set; }
    public long CompletionTokens { get; private set; }
    public long TotalTokens { get; private set; }
    public int RequestCount { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private UserTokenUsage()
    {
    }

    public UserTokenUsage(int userId)
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Add(int promptTokens, int completionTokens)
    {
        PromptTokens += promptTokens;
        CompletionTokens += completionTokens;
        TotalTokens += promptTokens + completionTokens;
        RequestCount++;
        UpdatedAt = DateTime.UtcNow;
    }
}
