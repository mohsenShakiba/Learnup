using Learnup.Application.Authentication;
using Learnup.Application.Helpers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Authentication;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Authentication;

public sealed record VerifyOtp(string MobileNumber, string Code) : IRequest<VerifyOtpResponse?>;

internal sealed class VerifyOtpHandler(ILearnupDbContext dbContext, IJwtTokenService jwtTokenService)
    : IRequestHandler<VerifyOtp, VerifyOtpResponse?>
{
    public async Task<VerifyOtpResponse?> Handle(VerifyOtp request, CancellationToken cancellationToken)
    {
        var mobileNumber = AuthHelper.NormalizeMobileNumber(request.MobileNumber);
        var code = AuthHelper.NormalizeCode(request.Code);
        var now = DateTime.UtcNow;

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

        user.RecordLogin(now);
        otp.Consume(now);

        await dbContext.SaveChangesAsync(cancellationToken);

        var token = jwtTokenService.CreateToken(user);
        return new VerifyOtpResponse(token.AccessToken, token.ExpiresAt);
    }

 
}
