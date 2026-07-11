namespace Learnup.Domain.AggregateRoots.Conversations;

public class ConversationItem
{
    public int Id { get; private set; }
    public string Content { get; private set; }
    public string? Translation { get; private set; }
    public int Order { get; private set; }
    public int Person { get; private set; }

    public int ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    public List<ConversationItemExpression> Expressions { get; private set; }
    public List<ConversationItemVoiceTiming> VoiceTimings { get; private set; }

    private ConversationItem()
    {
        Content = string.Empty;
        Translation = string.Empty;
        Expressions = [];
        VoiceTimings = [];
    }

    public ConversationItem(string content, string? translation, int person, int order)
    {
        Content = content;
        Translation = translation ?? "";
        Order = order;
        Person = person;
        Expressions = [];
        VoiceTimings = [];
    }

    /// <summary>
    /// Replaces the sentence-level voice timings for this item. Each tuple is a sentence's
    /// text with its start/end offset (seconds) into the generated audio.
    /// </summary>
    public void SetVoiceTimings(IEnumerable<(string Text, double Start, double End)> sentences)
    {
        VoiceTimings.Clear();

        var order = 0;
        foreach (var sentence in sentences)
        {
            VoiceTimings.Add(new ConversationItemVoiceTiming(sentence.Text, sentence.Start, sentence.End, order++));
        }
    }

    public void SetTranslation(string translation)
    {
        Translation = translation;
    }

    public void AddExpression(ConversationItemExpression expression)
    {
        Expressions.Add(expression);
    }
}
