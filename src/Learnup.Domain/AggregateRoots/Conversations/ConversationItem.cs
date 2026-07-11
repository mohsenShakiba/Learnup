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

    private ConversationItem()
    {
        Content = string.Empty;
        Translation = string.Empty;
        Expressions = [];
    }

    public ConversationItem(string content, string? translation, int person, int order)
    {
        Content = content;
        Translation = translation ?? "";
        Order = order;
        Person = person;
        Expressions = [];
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
