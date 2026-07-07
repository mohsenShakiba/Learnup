namespace Learnup.Domain.AggregateRoots.Conversations;

using Learnup.Domain.AggregateRoots.Users;

/// <summary>
/// A chat conversation between a user and the AI assistant.
/// Owns the ordered list of messages exchanged in both directions.
/// </summary>
public class Conversation
{
    public int Id { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string? Title { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<ChatMessage> _messages = [];
    public IReadOnlyList<ChatMessage> Messages => _messages.AsReadOnly();

    private Conversation()
    {
    }

    public Conversation(int userId)
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public ChatMessage AddMessage(ChatRole role, string content, int tokenCount = 0)
    {
        var message = new ChatMessage(this, role, content, tokenCount);

        _messages.Add(message);
        UpdatedAt = DateTime.UtcNow;

        if (role == ChatRole.User && string.IsNullOrWhiteSpace(Title))
        {
            Title = BuildTitle(content);
        }

        return message;
    }

    private static string BuildTitle(string content)
    {
        var trimmed = content.Trim();

        return trimmed.Length <= 50
            ? trimmed
            : trimmed[..50] + "…";
    }
}
