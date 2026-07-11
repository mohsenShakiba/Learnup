using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Conversations;

public class Conversation
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public ConversationStatus Status { get; private set; }
    public int? Duration { get; private set; }

    /// <summary>
    /// Stored audio file id for the whole conversation, generated in a single voice pass.
    /// The word-by-word timestamps are stored in a JSON file next to this audio.
    /// </summary>
    public string? VoiceId { get; private set; }

    public List<ConversationItem> Items { get; private set; }
    public List<LessonConversation> Lessons { get; private set; }

    private Conversation()
    {
        Title = string.Empty;
        Items = [];
    }

    public Conversation(string title)
    {
        Title = title;
        Items = [];
    }

    public void SetVoice(string voiceId)
    {
        VoiceId = voiceId;
    }

    public bool IsTranslated => Status.HasFlag(ConversationStatus.Translated);

    public bool IsVoiced => Status.HasFlag(ConversationStatus.Voiced);

    public void MarkAsVoiced()
    {
        Status |= ConversationStatus.Voiced;
    }

    public void MarkAsTranslated()
    {
        Status |= ConversationStatus.Translated;
    }
}
