using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Users;

public sealed record UpdateProfile(string DisplayName, string? AvatarUrl) : IRequest<UserProfileResponse?>;

internal sealed class UpdateProfileHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<UpdateProfile, UserProfileResponse?>
{
    public async Task<UserProfileResponse?> Handle(UpdateProfile request, CancellationToken cancellationToken)
    {
        var displayName = request.DisplayName.Trim();
        var avatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl)
            ? null
            : request.AvatarUrl.Trim();

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(request.DisplayName));
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == identityProvider.UserId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        user.UpdateProfile(displayName, avatarUrl);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserProfileResponse(
            user.Id,
            user.DisplayName,
            user.AvatarUrl,
            user.CreatedAt,
            user.LastLogin,
            user.Status);
    }
}
