using Learnup.Domain.AggregateRoots.Subscriptions;

namespace Learnup.Application.Responses.Public.Subscriptions;

public sealed record UserSubscriptionResponse(
    int Id,
    int SubscriptionId,
    string SubscriptionTitle,
    SubscriptionType SubscriptionType,
    SubscriptionDuration Duration,
    DateTime StartedAt,
    DateTime ExpiresAt,
    UserSubscriptionStatus Status);
