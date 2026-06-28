using System.Security.Cryptography;
using System.Text;
using Learnup.Application.ExternalServices;
using Learnup.Application.Helpers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Authentication;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Authentication;

public sealed record SendOtp(string MobileNumber) : IRequest<SendOtpResponse>;

internal sealed class SendOtpHandler(ILearnupDbContext dbContext, IOtpSender otpSender)
    : IRequestHandler<SendOtp, SendOtpResponse>
{
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(2);

    public async Task<SendOtpResponse> Handle(SendOtp request, CancellationToken cancellationToken)
    {
        var mobileNumber = AuthHelper.NormalizeMobileNumber(request.MobileNumber);
        var now = DateTime.UtcNow;
        var expiresAt = now.Add(OtpLifetime);
        var code = "1234";

        var activeOtpCode = await dbContext.UserOtps
            .Where(otp => otp.MobileNumber == mobileNumber)
            .OrderByDescending(otp => otp.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeOtpCode is not null && activeOtpCode.ConsumedAt is null)
        {
            return new SendOtpResponse(activeOtpCode.ExpiresAt);
        }

        var newOtpCode = new UserOtp(mobileNumber, code, now, expiresAt);
        
        dbContext.UserOtps.Add(newOtpCode);
        await otpSender.SendAsync(mobileNumber, code, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SendOtpResponse(expiresAt);
    }
    
}