namespace Learnup.Domain.AggregateRoots.Stories;

/// <summary>
/// An expression or phrase inside a <see cref="StoryItem"/> whose meaning might be
/// misunderstood by English learners (e.g. "it works for me", where "work" means "is suitable").
/// It captures the phrase and an explanation of what it means in the context of the story item.
/// </summary>
public class StoryItemExpression
{
    public int Id { get; private set; }

    /// <summary>The exact phrase as it appears in the story item, e.g. "it works for me".</summary>
    public string Phrase { get; private set; }

    /// <summary>An explanation of what the phrase means in this context.</summary>
    public string Meaning { get; private set; }

    /// <summary>Farsi translation of the phrase's contextual meaning.</summary>
    public string? Translation { get; private set; }

    public int StoryItemId { get; private set; }
    public StoryItem StoryItem { get; private set; } = null!;

    private StoryItemExpression()
    {
        Phrase = string.Empty;
        Meaning = string.Empty;
    }

    public StoryItemExpression(string phrase, string meaning, string? translation)
    {
        Phrase = phrase;
        Meaning = meaning;
        Translation = translation;
    }
}
