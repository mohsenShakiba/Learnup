namespace Learnup.Domain.AggregateRoots.AudioBooks;

public class AudioBookListItem
{
    public int Id { get; private set; }
    public string Sentence { get; private set; }
    public string? Translation { get; private set; }
    public int Order { get; private set; }
    public int AudioBookId { get; private set; }
    public AudioBooks AudioBook { get; private set; } = null!;
    public List<AudioBookListItemExpression> Expressions { get; private set; }

    private AudioBookListItem()
    {
        Sentence = string.Empty;
        Expressions = [];
    }

    public AudioBookListItem(string sentence, string? translation, int order)
    {
        Sentence = sentence;
        Translation = translation;
        Order = order;
        Expressions = [];
    }
}
