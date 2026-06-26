using Learnup.Application.Features.Public.MotivationalSentences;
using Learnup.Application.Features.Public.Users;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.MotivationalSentences;
using Learnup.Application.Responses.Public.Users;
using Learnup.API.Requests;
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

    [HttpPut("profile", Name = "UpdateProfile")]
    public async Task<ActionResult<UserProfileResponse>> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return BadRequest("Display name is required.");
        }

        if (request.DisplayName.Trim().Length > 100)
        {
            return BadRequest("Display name cannot exceed 100 characters.");
        }

        if (request.AvatarUrl?.Trim().Length > 500)
        {
            return BadRequest("Avatar URL cannot exceed 500 characters.");
        }

        var command = new UpdateProfile(request.DisplayName, request.AvatarUrl);
        var profile = await mediator.Send(command, cancellationToken);

        return profile is null
            ? NotFound()
            : Ok(profile);
    }

    [HttpPost("profile/avatar", Name = "UploadAvatar")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UserProfileResponse>> UploadAvatar(
        [FromForm] UploadAvatarRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length <= 0)
        {
            return BadRequest("Avatar file cannot be empty.");
        }

        if (!IsSupportedAvatarContentType(request.File.ContentType))
        {
            return BadRequest("Avatar file must be a JPEG, PNG, or WebP image.");
        }

        await using var avatarStream = request.File.OpenReadStream();
        var command = new UploadAvatar(
            avatarStream,
            request.File.ContentType,
            request.File.Length);

        var profile = await mediator.Send(command, cancellationToken);

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

    private static bool IsSupportedAvatarContentType(string contentType)
    {
        return contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase)
            || contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase)
            || contentType.Equals("image/webp", StringComparison.OrdinalIgnoreCase);
    }
}
