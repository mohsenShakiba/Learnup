namespace Learnup.Domain.AggregateRoots.Chats;

public class ChatMessage
{
    public int Id { get; private set; }

    public int ChatId { get; private set; }
    public Chat Chat { get; private set; } = null!;

    public ChatRole Role { get; private set; }
    public string Content { get; private set; }
    public int TokenCount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ChatMessage()
    {
        Content = null!;
    }

    public ChatMessage(Chat chat, ChatRole role, string content, int tokenCount = 0)
    {
        Chat = chat;
        Role = role;
        Content = content;
        TokenCount = tokenCount;
        CreatedAt = DateTime.UtcNow;
    }
}
