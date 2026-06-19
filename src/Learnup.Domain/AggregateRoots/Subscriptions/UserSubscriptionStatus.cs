namespace Learnup.Domain.AggregateRoots.Subscriptions;

public enum UserSubscriptionStatus
{
    Active = 1,
    Expired = -1,
    Cancelled = -2,
}
