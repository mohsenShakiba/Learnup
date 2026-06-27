using Learnup.Domain.AggregateRoots.Stories;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserStory
{
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int StoryId { get; private set; }
    public Story Story { get; private set; } = null!;

    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    
    public bool IsStoryCompleted { get; private set; }
    public bool IsGrammarCompleted { get; private set; }
    public bool IsVocabCompleted { get; private set; }

    public void CheckIfCompleted()
    {
        if (IsStoryCompleted && IsGrammarCompleted && IsVocabCompleted)
        {
            CompletedAt = DateTime.UtcNow;
        }
    }
}
