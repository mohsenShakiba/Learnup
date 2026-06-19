using Learnup.Application.Features.Public.Users;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Users;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class UsersController(IMediator mediator) : BasePublicController
{
    [HttpPost("streak", Name = "GetUserStreaks")]
    public async Task<ActionResult<UserStreakResponse>> GetUserStreaks(CancellationToken cancellationToken)
    {
        var query = new GetUserStreak();
        var streak = await mediator.Send(query, cancellationToken);
        return streak is null
            ? NotFound()
            : Ok(streak);
    }
}
