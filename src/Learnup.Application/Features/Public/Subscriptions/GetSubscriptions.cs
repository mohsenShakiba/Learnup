using Learnup.Application.Mappers;
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
            .Select(SubscriptionMapper.ToResponse())
            .ToListAsync(cancellationToken);
    }
}
