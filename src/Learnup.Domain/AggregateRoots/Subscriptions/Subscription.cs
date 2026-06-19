namespace Learnup.Domain.AggregateRoots.Subscriptions;

public class Subscription
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; } = null!;
    public SubscriptionType Type { get; private set; }
    public SubscriptionDuration Duration { get; private set; }
    public decimal Price { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<SubscriptionFeature> Features { get; private set; } = new List<SubscriptionFeature>();
    public ICollection<UserSubscription> UserSubscriptions { get; private set; } = new List<UserSubscription>();
}
