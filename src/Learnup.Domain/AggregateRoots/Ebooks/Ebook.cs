using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Domain.AggregateRoots.Ebooks;

public class Ebook
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string? Author { get; private set; }
    public string FileName { get; private set; }
    public string? CoverId { get; private set; }
    public DateTime UploadedAt { get; private set; }

    public ICollection<UserBook> Users { get; private set; } = new List<UserBook>();

    private Ebook()
    {
        Title = string.Empty;
        FileName = string.Empty;
    }

    public Ebook(string title, string fileName, string? coverId = null, string? author = null)
    {
        Title = title;
        Author = author;
        FileName = fileName;
        CoverId = coverId;
        UploadedAt = DateTime.UtcNow;
    }
}
