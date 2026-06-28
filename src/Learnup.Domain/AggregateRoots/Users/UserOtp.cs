namespace Learnup.Domain.AggregateRoots.Users;

public class UserOtp
{
    public int Id { get; private set; }
    public string MobileNumber { get; private set; }
    public string Code { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? ConsumedAt { get; private set; }

    private UserOtp()
    {
        MobileNumber = null!;
        Code = null!;
    }

    public UserOtp(string mobileNumber, string code, DateTime createdAt, DateTime expiresAt)
    {
        MobileNumber = mobileNumber;
        Code = code;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public bool IsValid(string code, DateTime now)
    {
        return ConsumedAt is null &&
               ExpiresAt >= now &&
               Code == code;
    }

    public void Consume(DateTime consumedAt)
    {
        ConsumedAt = consumedAt;
    }
}
