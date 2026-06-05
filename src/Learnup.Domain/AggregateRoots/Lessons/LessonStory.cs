using Learnup.Domain.AggregateRoots.Stories;

namespace Learnup.Domain.AggregateRoots.Lessons;

public class LessonStory
{
    public int LessonId { get; private set; }
    public int StoryId { get; private set; }
    
    public Lesson Lesson { get; private set; }
    public Story Story { get; private set; }

    private LessonStory()
    {
        Lesson = null!;
        Story = null!;
    }

    public LessonStory(int lessonId, int storyId)
    {
        LessonId = lessonId;
        StoryId = storyId;
        Lesson = null!;
        Story = null!;
    }
}
