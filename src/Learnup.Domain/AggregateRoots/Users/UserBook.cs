using Learnup.Domain.AggregateRoots.Ebooks;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserBook
{
    public int Id { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int EbookId { get; private set; }
    public Ebook Ebook { get; private set; } = null!;

    public string? CurrentRef { get; private set; }
    public float? Progress { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private UserBook()
    {
    }

    public UserBook(int userId, int ebookId)
    {
        UserId = userId;
        EbookId = ebookId;
        CreatedAt = DateTime.UtcNow;
    }

    public UserBook(int userId, Ebook ebook)
    {
        UserId = userId;
        Ebook = ebook;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateCurrentRef(string currentRef, float? progress)
    {
        CurrentRef = currentRef;
        Progress = progress;
    }
}
