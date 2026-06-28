namespace Learnup.Domain.AggregateRoots.Stories;

public class StoryItem
{
    public int Id { get; private set; }
    public string Content { get; private set; }
    public string Translation { get; private set; }
    public int Order { get; private set; }
    public int Person { get; private set; }
    public string? VoiceId { get; private set; }
    
    public int StoryId { get; private set; }
    public Story Story { get; private set; } = null!;

    private StoryItem()
    {
        Content = string.Empty;
        Translation = string.Empty;
    }

    public StoryItem(string content, string translation, int person, int order)
    {
        Content = content;
        Translation = translation;
        Order = order;
        Person = person;
    }

    public void SetVoice(string voiceId)
    {
        VoiceId = voiceId;
    }
}
