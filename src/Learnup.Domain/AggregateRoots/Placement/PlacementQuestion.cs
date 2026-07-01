namespace Learnup.Domain.AggregateRoots.Placement;

public class PlacementQuestion
{
    public int Id { get; private set; }
    public int PlacementTestId { get; private set; }
    public int Number { get; private set; }
    public string Level { get; private set; } = null!;
    public PlacementSkill Skill { get; private set; }
    public string Prompt { get; private set; } = null!;

    private readonly List<PlacementOption> _options = [];
    public IReadOnlyList<PlacementOption> Options => _options.AsReadOnly();

    private PlacementQuestion()
    {
    }

    public PlacementQuestion(int number, string level, PlacementSkill skill, string prompt)
    {
        Number = number;
        Level = level;
        Skill = skill;
        Prompt = prompt;
    }

    public void AddOption(PlacementOption option)
    {
        _options.Add(option);
    }
}
