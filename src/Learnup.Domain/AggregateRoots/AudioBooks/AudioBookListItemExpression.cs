namespace Learnup.Domain.AggregateRoots.AudioBooks;

public class AudioBookListItemExpression
{
    public int Id { get; private set; }
    public string Phrase { get; private set; }
    public string Meaning { get; private set; }
    public string? Translation { get; private set; }

    public int AudioBookListItemId { get; private set; }
    public AudioBookListItem AudioBookListItem { get; private set; } = null!;

    private AudioBookListItemExpression()
    {
        Phrase = string.Empty;
        Meaning = string.Empty;
    }
}
