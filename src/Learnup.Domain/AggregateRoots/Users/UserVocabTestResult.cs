using Learnup.Domain.AggregateRoots.Tests;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserVocabTestResult
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public int VocabTestId { get; private set; }
    public VocabTest VocabTest { get; private set; } = null!;
    public int SelectedOptionId { get; private set; }
    public VocabTestOption SelectedOption { get; private set; } = null!;
    public bool IsCorrect { get; private set; }
    public DateTime AnsweredAt { get; private set; }

    private UserVocabTestResult() { }

    public UserVocabTestResult(int userId, int vocabTestId, int selectedOptionId, bool isCorrect)
    {
        UserId = userId;
        VocabTestId = vocabTestId;
        SelectedOptionId = selectedOptionId;
        IsCorrect = isCorrect;
        AnsweredAt = DateTime.UtcNow;
    }
}
