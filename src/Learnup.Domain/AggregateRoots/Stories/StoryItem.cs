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
    public List<StoryItemVoiceTiming> VoiceTimings { get; private set; }

    private StoryItem()
    {
        Content = string.Empty;
        Translation = string.Empty;
        Expressions = [];
        VoiceTimings = [];
    }

    public StoryItem(string content, string? translation, int person, int order)
    {
        Content = content;
        Translation = translation ?? "";
        Order = order;
        Person = person;
        Expressions = [];
        VoiceTimings = [];
    }

    public void SetVoice(string voiceId)
    {
        VoiceId = voiceId;
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
            VoiceTimings.Add(new StoryItemVoiceTiming(sentence.Text, sentence.Start, sentence.End, order++));
        }
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
