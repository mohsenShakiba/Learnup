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
    public bool IsGrammarTestCompleted { get; private set; }
    public bool IsVocabTestCompleted { get; private set; }

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

    public void SetRequirements(int storiesCount, int grammarsCount, int vocabsCount)
    {
        if (storiesCount == 0)
        {
            IsStoryCompleted = true;
        }

        if (grammarsCount == 0)
        {
            IsGrammarCompleted = true;
            IsGrammarTestCompleted = true;
        }

        if (vocabsCount == 0)
        {
            IsVocabCompleted = true;
            IsVocabTestCompleted = true;
        }
    }

    public void CompleteStory()
    {
        IsStoryCompleted = true;
        Touch();
        CompleteIfReady();
    }
    
    public void CompleteGrammar()
    {
        IsGrammarCompleted = true;
        Touch();
        CompleteIfReady();
    }
    
    public void CompleteGrammarTest()
    {
        IsGrammarCompleted = true;
        IsGrammarTestCompleted = true;
        Touch();
        CompleteIfReady();
    }
    
    public void CompleteVocab()
    {
        IsVocabCompleted = true;
        Touch();
        CompleteIfReady();
    }
    
    public void CompleteVocabTest()
    {
        IsVocabCompleted = true;
        IsVocabTestCompleted = true;
        Touch();
        CompleteIfReady();
    }
}