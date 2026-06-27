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

    public void MarkAsCompleted()
    {
        Status = StoryStatus.Completed;
    }
}
