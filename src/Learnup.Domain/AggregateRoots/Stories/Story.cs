using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Stories;

public class Story
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public string? CoverId { get; private set; }
    public StoryStatus Status { get; private set; }
    public int? Duration { get; private set; }

    /// <summary>
    /// Stored audio file id for the whole conversation, generated in a single voice pass.
    /// The word-by-word timestamps are stored in a JSON file next to this audio.
    /// </summary>
    public string? VoiceId { get; private set; }

    public List<StoryItem> Items { get; private set; }
    public List<LessonStory> Lessons { get; private set; }

    private Story()
    {
        Title = string.Empty;
        Items = [];
    }

    public Story(string title)
    {
        Title = title;
        Items = [];
    }

    public void SetVoice(string voiceId)
    {
        VoiceId = voiceId;
    }

    public bool IsTranslated => Status.HasFlag(StoryStatus.Translated);

    public bool IsVoiced => Status.HasFlag(StoryStatus.Voiced);

    public void MarkAsVoiced()
    {
        Status |= StoryStatus.Voiced;
    }

    public void MarkAsTranslated()
    {
        Status |= StoryStatus.Translated;
    }
}
