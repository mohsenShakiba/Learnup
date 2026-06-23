namespace Learnup.Domain.AggregateRoots.Users;

public class UserBook
{
    public int Id { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Title { get; private set; }
    public string? Author { get; private set; }
    public string FileName { get; private set; }
    public string? CoverId { get; private set; }
    public string? CurrentRef { get; private set; }
    public float? Progress { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private UserBook()
    {
        Title = string.Empty;
        FileName = string.Empty;
    }

    public UserBook(int userId, string title, string fileName, string? coverId = null)
    {
        UserId = userId;
        Title = title;
        FileName = fileName;
        CoverId = coverId;
        UploadedAt = DateTime.UtcNow;
    }

    public void UpdateCurrentRef(string currentRef, float? progress)
    {
        CurrentRef = currentRef;
        Progress = progress;
    }
}
