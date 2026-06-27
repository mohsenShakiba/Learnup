using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Tests;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserLesson
{
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = null!;

    public DateTime StartedAt { get; private set; }
    public DateTime LastVisitedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    
    public bool IsStoryCompleted { get; private set; }
    public bool IsGrammarCompleted { get; private set; }
    public bool IsVocabCompleted { get; private set; }
    
    public UserLesson(int userId, int lessonId)
    {
        UserId = userId;
        LessonId = lessonId;
        StartedAt = LastVisitedAt = DateTime.UtcNow;
    }

    public void Touch()
    {
        LastVisitedAt = DateTime.UtcNow;
    }

    public void CompleteTest(TestType type)
    {
        if (type == TestType.Vocab)
        {
            IsVocabCompleted = true;
        }
        else if (type == TestType.Grammar)
        {
            IsGrammarCompleted = true;
        }

        Touch();
        CompleteIfReady();
    }

    private void CompleteIfReady()
    {
        if (IsStoryCompleted && IsGrammarCompleted && IsVocabCompleted)
        {
            CompletedAt ??= DateTime.UtcNow;
        }
    }
}
