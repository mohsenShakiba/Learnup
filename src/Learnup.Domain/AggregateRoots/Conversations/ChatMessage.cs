namespace Learnup.Domain.AggregateRoots.Conversations;

public class ChatMessage
{
    public int Id { get; private set; }

    public int ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    public ChatRole Role { get; private set; }
    public string Content { get; private set; }
    public int TokenCount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ChatMessage()
    {
        Content = null!;
    }

    public ChatMessage(Conversation conversation, ChatRole role, string content, int tokenCount = 0)
    {
        Conversation = conversation;
        Role = role;
        Content = content;
        TokenCount = tokenCount;
        CreatedAt = DateTime.UtcNow;
    }
}
