namespace Learnup.Domain.AggregateRoots.Placement;

public class PlacementTest
{
    public int Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Instructions { get; private set; } = null!;

    private readonly List<PlacementQuestion> _questions = [];
    public IReadOnlyList<PlacementQuestion> Questions => _questions.AsReadOnly();

    private PlacementTest()
    {
    }

    public PlacementTest(string title, string description, string instructions)
    {
        Title = title;
        Description = description;
        Instructions = instructions;
    }

    public void AddQuestion(PlacementQuestion question)
    {
        _questions.Add(question);
    }
}
