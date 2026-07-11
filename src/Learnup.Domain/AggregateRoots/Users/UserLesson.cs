using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Users;

[Flags]
public enum UserLessonStatus
{
    None = 0,
    ConversationCompleted = 1,
    GrammarCompleted = 2,
    VocabCompleted = 4,
    TestCompleted = 8
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

    public bool HasConversation { get; private set; }
    public bool HasGrammar { get; private set; }
    public bool HasVocab { get; private set; }
    public bool HasTest { get; private set; }

    public bool IsConversationCompleted => Status.HasFlag(UserLessonStatus.ConversationCompleted);
    public bool IsGrammarCompleted => Status.HasFlag(UserLessonStatus.GrammarCompleted);
    public bool IsVocabCompleted => Status.HasFlag(UserLessonStatus.VocabCompleted);
    public bool IsTestCompleted => Status.HasFlag(UserLessonStatus.TestCompleted);

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
        if (IsConversationCompleted && IsGrammarCompleted && IsVocabCompleted)
        {
            CompletedAt ??= DateTime.UtcNow;
        }
    }

    public bool IsCompleted()
    {
        return IsConversationCompleted && IsGrammarCompleted && IsVocabCompleted;
    }

    public void SetRequirements(int conversationsCount, int grammarsCount, int vocabsCount, int testsCount)
    {
        if (conversationsCount > 0)
        {
            HasConversation = true;
        }
        else
        {
            Complete(UserLessonStatus.ConversationCompleted);
        }

        if (grammarsCount > 0)
        {
            HasGrammar = true;
        }
        else
        {
            Complete(UserLessonStatus.GrammarCompleted);
        }

        if (vocabsCount > 0)
        {
            HasVocab = true;
        }
        else
        {
            Complete(UserLessonStatus.VocabCompleted);
        }

        if (testsCount > 0)
        {
            HasTest = true;
        }
        else
        {
            Complete(UserLessonStatus.TestCompleted);
        }
    }


    public void Complete(UserLessonStatus status)
    {
        Status |= status;
        Touch();
        CompleteIfReady();
    }


}
