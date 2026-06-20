namespace Learnup.Domain.AggregateRoots.MotivationalSentences;

public class MotivationalSentence
{
    public int Id { get; private set; }
    public string Sentence { get; private set; } = null!;
    public bool IsActive { get; private set; }
}
