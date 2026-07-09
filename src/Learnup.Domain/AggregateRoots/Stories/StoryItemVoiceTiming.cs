namespace Learnup.Domain.AggregateRoots.Stories;

/// <summary>
/// A sentence-level timing marker for a <see cref="StoryItem"/>'s generated voice audio,
/// describing when a sentence starts and ends within the audio file (in seconds).
/// Used to highlight the currently spoken sentence during playback.
/// </summary>
public class StoryItemVoiceTiming
{
    public int Id { get; private set; }

    /// <summary>Zero-based position of the sentence within the story item's audio.</summary>
    public int Order { get; private set; }

    /// <summary>The sentence text this timing covers.</summary>
    public string Text { get; private set; }

    /// <summary>Offset (seconds) into the audio where the sentence starts.</summary>
    public double StartSeconds { get; private set; }

    /// <summary>Offset (seconds) into the audio where the sentence ends.</summary>
    public double EndSeconds { get; private set; }

    public int StoryItemId { get; private set; }
    public StoryItem StoryItem { get; private set; } = null!;

    private StoryItemVoiceTiming()
    {
        Text = string.Empty;
    }

    public StoryItemVoiceTiming(string text, double startSeconds, double endSeconds, int order)
    {
        Text = text;
        StartSeconds = startSeconds;
        EndSeconds = endSeconds;
        Order = order;
    }
}
