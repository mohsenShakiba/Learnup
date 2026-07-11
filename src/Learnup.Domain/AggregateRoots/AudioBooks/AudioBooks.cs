namespace Learnup.Domain.AggregateRoots.AudioBooks;

public class AudioBooks
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public string? Author { get; private set; }
    public string? Level { get; private set; }
    public string? Year { get; private set; }
    public string? WordCount { get; private set; }
    public string? Source { get; private set; }
    public string? Content { get; private set; }
    public AudioBookStatus Status { get; private set; }
    
    public string? CoverId { get; private set; }
    public string? VoiceId { get; private set; }
    public string? TimingJsonId { get; private set; }
    public List<AudioBookListItem> Items { get; private set; }

    private AudioBooks()
    {
        Title = string.Empty;
        Items = [];
    }

    public AudioBooks(
        string title,
        string? description,
        string? author,
        string? level,
        string? year,
        string? wordCount,
        string? source,
        string content)
    {
        Title = title;
        Description = description;
        Author = author;
        Level = level;
        Year = year;
        WordCount = wordCount;
        Source = source;
        Content = content;
        Items = [];
    }

    public void AddItem(AudioBookListItem item)
    {
        Items.Add(item);
    }
}
