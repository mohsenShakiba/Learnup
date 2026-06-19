using Learnup.Application.Features.Public.Subscriptions;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class SubscriptionsController(IMediator mediator) : BasePublicController
{
    [HttpGet(Name = "GetSubscriptions")]
    public async Task<ActionResult<IReadOnlyList<SubscriptionResponse>>> GetSubscriptions(
        CancellationToken cancellationToken)
    {
        var subscriptions = await mediator.Send(new GetSubscriptions(), cancellationToken);

        return Ok(subscriptions);
    }

    [HttpGet("me", Name = "GetUserCurrentSubscription")]
    public async Task<ActionResult<UserSubscriptionResponse>> GetCurrentUserSubscription(
        CancellationToken cancellationToken)
    {
        var subscription = await mediator.Send(new GetUserCurrentSubscription(), cancellationToken);

        return subscription is null
            ? NotFound()
            : Ok(subscription);
    }
}
