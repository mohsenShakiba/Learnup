using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Domain.AggregateRoots.Grammars;

public class Grammar
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public VocabLevel Level { get; private set; }
    public int Order { get; private set; }
    public TimeSpan EstimatedTime { get; private set; }
    public string Description { get; private set; }
    
    public int? ParentGrammarId { get; private set; }
    public Grammar? ParentGrammar { get; private set; } 

    public IReadOnlyList<Grammar> PrerequisiteGrammars => _prerequisiteGrammars.AsReadOnly();
    private List<Grammar> _prerequisiteGrammars = [];

    public IReadOnlyList<GrammarLesson> Lessons => _lessons.AsReadOnly();
    private List<GrammarLesson> _lessons = [];

    public Grammar(string name, VocabLevel level, int order, TimeSpan estimatedTime, string description, int? parentGrammarId)
    {
        Name = name;
        Level = level;
        Order = order;
        EstimatedTime = estimatedTime;
        Description = description;
    }

    public void AddLesson(GrammarLesson lesson)
    {
        _lessons.Add(lesson);
    }

    public void RemoveLesson(GrammarLesson lesson)
    {
        _lessons.Remove(lesson);
    }

    public void AddGrammar(Grammar grammar)
    {
        _prerequisiteGrammars.Add(grammar);
    }

    public void RemoveGrammar(Grammar grammar)
    {
        _prerequisiteGrammars.Remove(grammar);
    }
}
