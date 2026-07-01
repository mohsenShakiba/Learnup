using Learnup.Application.Authentication;
using Learnup.Application.Helpers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Authentication;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Authentication;

public sealed record CompleteSignup(
    string MobileNumber,
    string Code,
    string DisplayName,
    string? AvatarUrl) : IRequest<VerifyOtpResponse?>;

internal sealed class CompleteSignupHandler(ILearnupDbContext dbContext, IJwtTokenService jwtTokenService)
    : IRequestHandler<CompleteSignup, VerifyOtpResponse?>
{
    public async Task<VerifyOtpResponse?> Handle(CompleteSignup request, CancellationToken cancellationToken)
    {
        var mobileNumber = AuthHelper.NormalizeMobileNumber(request.MobileNumber);
        var code = AuthHelper.NormalizeCode(request.Code);
        var displayName = request.DisplayName.Trim();
        var avatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl)
            ? null
            : request.AvatarUrl.Trim();
        var now = DateTime.UtcNow;

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(request.DisplayName));
        }

        var otp = await dbContext.UserOtps
            .Where(item => item.MobileNumber == mobileNumber && item.ConsumedAt == null)
            .OrderByDescending(item => item.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null || !otp.IsValid(code, now))
        {
            return null;
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(item => item.MobileNumber == mobileNumber, cancellationToken);

        if (user is null)
        {
            user = new User(mobileNumber, now);
            dbContext.Users.Add(user);
        }

        user.UpdateProfile(displayName, avatarUrl);
        user.RecordLogin(now);
        otp.Consume(now);

        await dbContext.SaveChangesAsync(cancellationToken);

        var token = jwtTokenService.CreateToken(user);
        return VerifyOtpResponse.SignedIn(token.AccessToken, token.ExpiresAt);
    }
}
