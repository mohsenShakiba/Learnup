namespace Learnup.Domain.AggregateRoots.Users;

public class User
{
    public int Id { get; private set; }
    public string DisplayName { get; private set; }
    public string MobileNumber { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLogin { get; private set; }
    public UserStatus Status { get; private set; }
    
    public ICollection<UserLesson> Lessons { get; private set; } = new List<UserLesson>();
    public ICollection<UserBook> Books { get; private set; } = new List<UserBook>();

    private User()
    {
        DisplayName = null!;
        MobileNumber = null!;
    }

    public User(string mobileNumber, DateTime createdAt)
    {
        MobileNumber = mobileNumber;
        DisplayName = mobileNumber;
        CreatedAt = createdAt;
        Status = UserStatus.Active;
    }

    public void RecordLogin(DateTime loggedInAt)
    {
        LastLogin = loggedInAt;
    }

    public void UpdateProfile(string displayName, string? avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        DisplayName = displayName.Trim();
        AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();
    }

    public void UpdateAvatar(string avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(avatarUrl))
        {
            throw new ArgumentException("Avatar URL is required.", nameof(avatarUrl));
        }

        AvatarUrl = avatarUrl.Trim();
    }


}
