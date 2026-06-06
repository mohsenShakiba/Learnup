namespace Learnup.Domain.AggregateRoots.Stories;

public sealed class StoryItemTimestamp
{
    public int Id { get; private set; }

    public string Word { get; private set; }
    public float Start { get; private set; }
    public float End { get; private set; }
    
    public int StoryItemId { get; private set; }
    public StoryItem StoryItem { get; private set; } = null!;

    private StoryItemTimestamp()
    {
        Word = string.Empty;
    }

    public StoryItemTimestamp(int storyItemId, string word, float start, float end)
    {
        StoryItemId = storyItemId;
        Word = word;
        Start = start;
        End = end;
    }
}
