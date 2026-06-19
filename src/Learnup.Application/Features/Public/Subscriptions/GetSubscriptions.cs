using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Subscriptions;

public sealed record GetSubscriptions : IRequest<IReadOnlyList<SubscriptionResponse>>;

internal sealed class GetSubscriptionsHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetSubscriptions, IReadOnlyList<SubscriptionResponse>>
{
    public async Task<IReadOnlyList<SubscriptionResponse>> Handle(
        GetSubscriptions request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Id)
            .Select(s => new SubscriptionResponse(
                s.Id,
                s.Title,
                s.Description,
                s.Type,
                s.Duration,
                s.Price,
                s.DiscountPercent,
                s.IsActive,
                s.Features
                    .OrderBy(f => f.Order)
                    .Select(f => new SubscriptionFeatureResponse(f.Id, f.Description, f.IsIncluded, f.Order))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}
