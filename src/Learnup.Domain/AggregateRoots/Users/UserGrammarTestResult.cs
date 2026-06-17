using Learnup.Domain.AggregateRoots.Tests;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserGrammarTestResult
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public int GrammarTestId { get; private set; }
    public GrammarTest GrammarTest { get; private set; } = null!;
    public int SelectedOptionId { get; private set; }
    public GrammarTestOption SelectedOption { get; private set; } = null!;
    public bool IsCorrect { get; private set; }
    public DateTime AnsweredAt { get; private set; }

    private UserGrammarTestResult() { }

    public UserGrammarTestResult(int userId, int grammarTestId, int selectedOptionId, bool isCorrect)
    {
        UserId = userId;
        GrammarTestId = grammarTestId;
        SelectedOptionId = selectedOptionId;
        IsCorrect = isCorrect;
        AnsweredAt = DateTime.UtcNow;
    }
}
