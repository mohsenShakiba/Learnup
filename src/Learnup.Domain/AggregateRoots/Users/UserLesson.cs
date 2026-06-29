using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Users;

[Flags]
public enum UserLessonStatus
{
    None = 0,
    StoryCompleted = 1,
    GrammarCompleted = 2,
    GrammarTestCompleted = 4,
    VocabCompleted = 8,
    VocabTestCompleted = 16,
}

public class UserLesson
{
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = null!;

    public DateTime StartedAt { get; private set; }
    public DateTime LastVisitedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public UserLessonStatus Status { get; private set; } = UserLessonStatus.None;

    public bool HasStory { get; private set; }
    public bool HasGrammar { get; private set; }
    public bool HasVocab { get; private set; }
    public bool HasTest { get; private set; }

    public bool IsStoryCompleted => Status.HasFlag(UserLessonStatus.StoryCompleted);
    public bool IsGrammarCompleted => Status.HasFlag(UserLessonStatus.GrammarCompleted);
    public bool IsGrammarTestCompleted => Status.HasFlag(UserLessonStatus.GrammarTestCompleted);
    public bool IsVocabCompleted => Status.HasFlag(UserLessonStatus.VocabCompleted);
    public bool IsVocabTestCompleted => Status.HasFlag(UserLessonStatus.VocabTestCompleted);

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

    private void CompleteIfReady()
    {
        if (IsStoryCompleted && IsGrammarCompleted && IsVocabCompleted)
        {
            CompletedAt ??= DateTime.UtcNow;
        }
    }

    public bool IsCompleted()
    {
        return IsStoryCompleted && IsGrammarCompleted && IsVocabCompleted;
    }

    public void SetRequirements(int storiesCount, int grammarsCount, int vocabsCount, int testsCount)
    {
        if (storiesCount > 0)
        {
            HasStory = true;
        }
        else
        {
            Complete(UserLessonStatus.StoryCompleted);
        }

        if (grammarsCount > 0)
        {
            HasGrammar = true;
        }
        else
        {
            Complete(UserLessonStatus.GrammarCompleted | UserLessonStatus.GrammarTestCompleted);
        }

        if (vocabsCount > 0)
        {
            HasVocab = true;
        }
        else
        {
            Complete(UserLessonStatus.VocabCompleted | UserLessonStatus.VocabTestCompleted);
        }

        if (testsCount > 0)
        {
            HasTest = true;
        }
        else
        {
            Complete(UserLessonStatus.GrammarTestCompleted | UserLessonStatus.VocabTestCompleted);
        }
    }

    public void CompleteStory()
    {
        Complete(UserLessonStatus.StoryCompleted);
        Touch();
        CompleteIfReady();
    }
    
    public void CompleteGrammar()
    {
        Complete(UserLessonStatus.GrammarCompleted);
        Touch();
        CompleteIfReady();
    }

    public void CompleteGrammarTest()
    {
        Complete(UserLessonStatus.GrammarCompleted | UserLessonStatus.GrammarTestCompleted);
        Touch();
        CompleteIfReady();
    }
    
    public void CompleteVocab()
    {
        Complete(UserLessonStatus.VocabCompleted);
        Touch();
        CompleteIfReady();
    }
    
    public void CompleteVocabTest()
    {
        Complete(UserLessonStatus.VocabCompleted | UserLessonStatus.VocabTestCompleted);
        Touch();
        CompleteIfReady();
    }

    private void Complete(UserLessonStatus status)
    {
        Status |= status;
    }
}
