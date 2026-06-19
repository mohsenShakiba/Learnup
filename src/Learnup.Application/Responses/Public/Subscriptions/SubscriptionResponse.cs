using Learnup.Domain.AggregateRoots.Subscriptions;

namespace Learnup.Application.Responses.Public.Subscriptions;

public sealed record SubscriptionResponse(
    int Id,
    string Title,
    string Description,
    SubscriptionType Type,
    SubscriptionDuration Duration,
    decimal Price,
    decimal DiscountPercent,
    bool IsActive,
    IReadOnlyList<SubscriptionFeatureResponse> Features);

public sealed record SubscriptionFeatureResponse(
    int Id,
    string Description,
    bool IsIncluded,
    int Order);
