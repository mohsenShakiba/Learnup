namespace Learnup.Domain.AggregateRoots.Subscriptions;

public class SubscriptionFeature
{
    public int Id { get; private set; }
    public int SubscriptionId { get; private set; }
    public Subscription Subscription { get; private set; } = null!;

    public string Description { get; private set; }
    public bool IsIncluded { get; private set; }
    public int Order { get; private set; }

    public SubscriptionFeature(int subscriptionId, string description, bool isIncluded, int order)
    {
        SubscriptionId = subscriptionId;
        Description = description;
        IsIncluded = isIncluded;
        Order = order;
    }
}
