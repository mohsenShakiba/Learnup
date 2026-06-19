using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Subscriptions;
using Learnup.Domain.AggregateRoots.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Subscriptions;

public sealed record GetUserCurrentSubscription : IRequest<UserSubscriptionResponse?>;

internal sealed class GetUserCurrentSubscriptionHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<GetUserCurrentSubscription, UserSubscriptionResponse?>
{
    public async Task<UserSubscriptionResponse?> Handle(
        GetUserCurrentSubscription request,
        CancellationToken cancellationToken)
    {
        return await dbContext.UserSubscriptions
            .AsNoTracking()
            .Where(us => us.UserId == identityProvider.UserId && us.Status == UserSubscriptionStatus.Active)
            .OrderByDescending(us => us.StartedAt)
            .Select(us => new UserSubscriptionResponse(
                us.Id,
                us.SubscriptionId,
                us.Subscription.Title,
                us.Subscription.Type,
                us.Subscription.Duration,
                us.StartedAt,
                us.ExpiresAt,
                us.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
