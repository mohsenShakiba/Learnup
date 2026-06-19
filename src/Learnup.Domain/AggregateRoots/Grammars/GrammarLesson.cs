using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Grammars;

public class GrammarLesson
{
    public int Id { get; private set; }
    public string Title { get; private set; }

    public HTMLTag HtmlTag { get; private set; }
    public string Content { get; private set; }
    public int Order { get; private set; }

    public string Language { get; private set; }
    
    public int GrammarId { get; private set; }
    public Grammar Grammar { get; private set; }
    
    public int? VoiceId { get; private set; }

    public GrammarLesson(string title, HTMLTag htmlTag, string content, int order, string language, int? voiceId, int grammarId)
    {
        Title = title;
        HtmlTag = htmlTag;
        Content = content;
        Order = order;
        Language = language;
        VoiceId = voiceId;
        GrammarId = grammarId;
        Grammar = null!;
    }
}
