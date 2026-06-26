namespace Learnup.Domain.AggregateRoots.Users;

public class UserOtp
{
    public int Id { get; private set; }
    public string MobileNumber { get; private set; }
    public string CodeHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? ConsumedAt { get; private set; }

    private UserOtp()
    {
        MobileNumber = null!;
        CodeHash = null!;
    }

    public UserOtp(string mobileNumber, string codeHash, DateTime createdAt, DateTime expiresAt)
    {
        MobileNumber = mobileNumber;
        CodeHash = codeHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public bool IsValid(string codeHash, DateTime now)
    {
        return ConsumedAt is null &&
               ExpiresAt >= now &&
               CodeHash == codeHash;
    }

    public void Consume(DateTime consumedAt)
    {
        ConsumedAt = consumedAt;
    }
}
