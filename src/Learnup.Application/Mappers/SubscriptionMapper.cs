using System.Linq.Expressions;
using Learnup.Application.Responses.Public.Subscriptions;
using Learnup.Domain.AggregateRoots.Subscriptions;

namespace Learnup.Application.Mappers;

public static class SubscriptionMapper
{
    public static Expression<Func<Subscription, SubscriptionResponse>> ToResponse() =>
        subscription => new SubscriptionResponse(
            subscription.Id,
            subscription.Title,
            subscription.Description,
            subscription.Type,
            subscription.Duration,
            subscription.Price,
            subscription.DiscountPercent,
            subscription.IsActive,
            subscription.Features
                .OrderBy(feature => feature.Order)
                .Select(feature => new SubscriptionFeatureResponse(
                    feature.Id,
                    feature.Description,
                    feature.IsIncluded,
                    feature.Order))
                .ToList());

    public static Expression<Func<UserSubscription, UserSubscriptionResponse>> ToUserSubscriptionResponse() =>
        userSubscription => new UserSubscriptionResponse(
            userSubscription.Id,
            userSubscription.SubscriptionId,
            userSubscription.Subscription.Title,
            userSubscription.Subscription.Type,
            userSubscription.Subscription.Duration,
            userSubscription.StartedAt,
            userSubscription.ExpiresAt,
            userSubscription.Status);

    public static SubscriptionResponse ToResponse(this Subscription subscription) =>
        new(
            subscription.Id,
            subscription.Title,
            subscription.Description,
            subscription.Type,
            subscription.Duration,
            subscription.Price,
            subscription.DiscountPercent,
            subscription.IsActive,
            subscription.Features
                .OrderBy(feature => feature.Order)
                .Select(feature => feature.ToResponse())
                .ToList());

    public static SubscriptionFeatureResponse ToResponse(this SubscriptionFeature feature) =>
        new(
            feature.Id,
            feature.Description,
            feature.IsIncluded,
            feature.Order);

    public static UserSubscriptionResponse ToResponse(this UserSubscription userSubscription) =>
        new(
            userSubscription.Id,
            userSubscription.SubscriptionId,
            userSubscription.Subscription.Title,
            userSubscription.Subscription.Type,
            userSubscription.Subscription.Duration,
            userSubscription.StartedAt,
            userSubscription.ExpiresAt,
            userSubscription.Status);
}
