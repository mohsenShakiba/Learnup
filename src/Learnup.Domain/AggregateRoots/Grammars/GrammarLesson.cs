using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Grammars;

public class GrammarLesson
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    
    public HTMLTag HTMLTag { get; private set; }
    public string Content { get; private set; }
    public int Order { get; private set; }
    
    public string Language { get; private set; }
    
    public int? VoiceId { get; private set; }
}