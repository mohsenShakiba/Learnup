using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Users;

public sealed record GetProfile : IRequest<UserProfileResponse?>;

internal sealed class GetProfileHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<GetProfile, UserProfileResponse?>
{
    public async Task<UserProfileResponse?> Handle(
        GetProfile request,
        CancellationToken cancellationToken)
    {
        var profile = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == identityProvider.UserId)
            .Select(user => new UserProfileResponse(
                user.Id,
                user.DisplayName,
                user.AvatarUrl,
                user.CreatedAt,
                user.LastLogin,
                user.Status))
            .FirstOrDefaultAsync(cancellationToken);

        return profile;
    }
}
