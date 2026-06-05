using Learnup.Domain.AggregateRoots.Grammars;

namespace Learnup.Domain.AggregateRoots.Lessons;

public class LessonGrammar
{
    public int LessonId { get; private set; }
    public int GrammarId { get; private set; }
    
    public Lesson Lesson { get; private set; }
    public Grammar Grammar { get; private set; }

    private LessonGrammar()
    {
        Lesson = null!;
        Grammar = null!;
    }

    public LessonGrammar(int lessonId, int grammarId)
    {
        LessonId = lessonId;
        GrammarId = grammarId;
        Lesson = null!;
        Grammar = null!;
    }
}
