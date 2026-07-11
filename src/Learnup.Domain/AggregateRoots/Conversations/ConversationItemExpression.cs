namespace Learnup.Domain.AggregateRoots.Conversations;

/// <summary>
/// An expression or phrase inside a <see cref="ConversationItem"/> whose meaning might be
/// misunderstood by English learners (e.g. "it works for me", where "work" means "is suitable").
/// It captures the phrase and an explanation of what it means in the context of the conversation item.
/// </summary>
public class ConversationItemExpression
{
    public int Id { get; private set; }

    /// <summary>The exact phrase as it appears in the conversation item, e.g. "it works for me".</summary>
    public string Phrase { get; private set; }

    /// <summary>An explanation of what the phrase means in this context.</summary>
    public string Meaning { get; private set; }

    /// <summary>Farsi translation of the phrase's contextual meaning.</summary>
    public string? Translation { get; private set; }

    public int ConversationItemId { get; private set; }
    public ConversationItem ConversationItem { get; private set; } = null!;

    private ConversationItemExpression()
    {
        Phrase = string.Empty;
        Meaning = string.Empty;
    }

    public ConversationItemExpression(string phrase, string meaning, string? translation)
    {
        Phrase = phrase;
        Meaning = meaning;
        Translation = translation;
    }
}
