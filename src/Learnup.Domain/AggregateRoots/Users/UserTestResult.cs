using Learnup.Domain.AggregateRoots.Tests;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserTestResult
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public int TestId { get; private set; }
    public Test Test { get; private set; } = null!;
    public int SelectedOptionId { get; private set; }
    public TestOption SelectedOption { get; private set; } = null!;
    public bool IsCorrect { get; private set; }
    public DateTime AnsweredAt { get; private set; }

    private UserTestResult()
    {
    }

    public UserTestResult(int userId, int testId, int selectedOptionId, bool isCorrect)
    {
        UserId = userId;
        TestId = testId;
        SelectedOptionId = selectedOptionId;
        IsCorrect = isCorrect;
        AnsweredAt = DateTime.UtcNow;
    }

    public void UpdateSelectedOption(int selectedOptionId, bool isCorrect)
    {
        SelectedOptionId = selectedOptionId;
        IsCorrect = isCorrect;
    }
}
