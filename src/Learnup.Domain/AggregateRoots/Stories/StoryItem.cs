namespace Learnup.Domain.AggregateRoots.Stories;

public class StoryItem
{
    public int Id { get; private set; }
    public string Content { get; private set; }
    public string? Translation { get; private set; }
    public int Order { get; private set; }
    public int Person { get; private set; }
    public string? VoiceId { get; private set; }

    public int StoryId { get; private set; }
    public Story Story { get; private set; } = null!;

    public List<StoryItemExpression> Expressions { get; private set; }

    private StoryItem()
    {
        Content = string.Empty;
        Translation = string.Empty;
        Expressions = [];
    }

    public StoryItem(string content, string? translation, int person, int order)
    {
        Content = content;
        Translation = translation ?? "";
        Order = order;
        Person = person;
        Expressions = [];
    }

    public void SetVoice(string voiceId)
    {
        VoiceId = voiceId;
    }

    public void SetTranslation(string translation)
    {
        Translation = translation;
    }

    public void AddExpression(StoryItemExpression expression)
    {
        Expressions.Add(expression);
    }
}
