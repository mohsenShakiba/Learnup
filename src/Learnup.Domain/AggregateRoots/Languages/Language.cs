namespace Learnup.Domain.AggregateRoots.Languages;

public class Language
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public Language(int id, string name)
    {
        Id = id;
        Name = name;
    }
}