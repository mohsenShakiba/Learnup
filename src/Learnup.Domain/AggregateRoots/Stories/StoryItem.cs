namespace Learnup.Domain.AggregateRoots.Stories;

public class StoryItem
{
    public int Id { get; private set; }
    public string Content { get; private set; }
    public string Translation { get; private set; }
    public int Order { get; private set; }
    public string? VoiceId { get; private set; }
    public List<StoryItemTimestamp> Timestamps { get; private set; }
    
    public int StoryId { get; private set; }
    public Story Story { get; private set; } = null!;

    private StoryItem()
    {
        Content = string.Empty;
        Translation = string.Empty;
        Timestamps = [];
    }

    public StoryItem(string content, string translation, int order)
    {
        Content = content;
        Translation = translation;
        Order = order;
        Timestamps = [];
    }

    public void SetVoice(string voiceId, IEnumerable<StoryItemTimestamp> timestamps)
    {
        VoiceId = voiceId;
        Timestamps = timestamps.ToList();
    }
}
