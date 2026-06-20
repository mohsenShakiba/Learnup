using Learnup.Application.Features.Public.MotivationalSentences;
using Learnup.Application.Features.Public.Users;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.MotivationalSentences;
using Learnup.Application.Responses.Public.Users;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class UsersController(IMediator mediator) : BasePublicController
{
    [HttpPost("profile", Name = "GetProfile")]
    public async Task<ActionResult<UserProfileResponse>> GetProfile(CancellationToken cancellationToken)
    {
        var query = new GetProfile();
        var profile = await mediator.Send(query, cancellationToken);
        return profile is null
            ? NotFound()
            : Ok(profile);
    }

    [HttpPost("streak", Name = "GetUserStreaks")]
    public async Task<ActionResult<UserStreakResponse>> GetUserStreaks(CancellationToken cancellationToken)
    {
        var query = new GetUserStreak();
        var streak = await mediator.Send(query, cancellationToken);
        return streak is null
            ? NotFound()
            : Ok(streak);
    }
    
    [HttpGet(Name = "GetMotivationalSentence")]
    public async Task<ActionResult<MotivationalSentenceResponse>> GetMotivationalSentence(
        CancellationToken cancellationToken)
    {
        var sentence = await mediator.Send(new GetMotivationalSentence(), cancellationToken);

        return sentence is null
            ? NotFound()
            : Ok(sentence);
    }
}
