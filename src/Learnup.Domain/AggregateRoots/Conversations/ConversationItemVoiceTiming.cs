namespace Learnup.Domain.AggregateRoots.Conversations;

/// <summary>
/// A sentence-level timing marker for a <see cref="ConversationItem"/>'s generated voice audio,
/// describing when a sentence starts and ends within the audio file (in seconds).
/// Used to highlight the currently spoken sentence during playback.
/// </summary>
public class ConversationItemVoiceTiming
{
    public int Id { get; private set; }

    /// <summary>Zero-based position of the sentence within the conversation item's audio.</summary>
    public int Order { get; private set; }

    /// <summary>The sentence text this timing covers.</summary>
    public string Text { get; private set; }

    /// <summary>Offset (seconds) into the audio where the sentence starts.</summary>
    public double StartSeconds { get; private set; }

    /// <summary>Offset (seconds) into the audio where the sentence ends.</summary>
    public double EndSeconds { get; private set; }

    public int ConversationItemId { get; private set; }
    public ConversationItem ConversationItem { get; private set; } = null!;

    private ConversationItemVoiceTiming()
    {
        Text = string.Empty;
    }

    public ConversationItemVoiceTiming(string text, double startSeconds, double endSeconds, int order)
    {
        Text = text;
        StartSeconds = startSeconds;
        EndSeconds = endSeconds;
        Order = order;
    }
}
