using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Domain.AggregateRoots.Subscriptions;

public class UserSubscription
{
    public int Id { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int SubscriptionId { get; private set; }
    public Subscription Subscription { get; private set; } = null!;

    public DateTime StartedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public UserSubscriptionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public UserSubscription(int userId, int subscriptionId, DateTime startedAt, DateTime expiresAt)
    {
        UserId = userId;
        SubscriptionId = subscriptionId;
        StartedAt = startedAt;
        ExpiresAt = expiresAt;
        Status = UserSubscriptionStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }
}
