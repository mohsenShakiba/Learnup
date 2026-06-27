using Learnup.Domain.AggregateRoots.Grammars;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserGrammar
{
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    
    public int GrammarId { get; private set; }
    public Grammar Grammar { get; private set; } = null!;
    
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private UserGrammar()
    {
    }

    public UserGrammar(int userId, int grammarId)
    {
        UserId = userId;
        GrammarId = grammarId;
        StartedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        CompletedAt ??= DateTime.UtcNow;
    }
}
