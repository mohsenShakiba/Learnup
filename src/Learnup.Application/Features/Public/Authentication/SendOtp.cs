using System.Security.Cryptography;
using System.Text;
using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Authentication;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Authentication;

public sealed record SendOtp(string MobileNumber) : IRequest<SendOtpResponse>;

internal sealed class SendOtpHandler(
    ILearnupDbContext dbContext,
    IOtpSender otpSender)
    : IRequestHandler<SendOtp, SendOtpResponse>
{
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(2);

    public async Task<SendOtpResponse> Handle(SendOtp request, CancellationToken cancellationToken)
    {
        var mobileNumber = NormalizeMobileNumber(request.MobileNumber);
        var now = DateTime.UtcNow;
        var expiresAt = now.Add(OtpLifetime);
        var code = "1234";

        var activeOtps = await dbContext.UserOtps
            .Where(otp => otp.MobileNumber == mobileNumber && otp.ConsumedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var otp in activeOtps)
        {
            otp.Consume(now);
        }

        dbContext.UserOtps.Add(new UserOtp(mobileNumber, HashOtp(mobileNumber, code), now, expiresAt));

        await otpSender.SendAsync(mobileNumber, code, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SendOtpResponse(expiresAt);
    }

    internal static string NormalizeMobileNumber(string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new ArgumentException("Mobile number is required.", nameof(mobileNumber));
        }

        return mobileNumber.Trim();
    }

    internal static string HashOtp(string mobileNumber, string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{mobileNumber}:{code}"));
        return Convert.ToHexString(bytes);
    }
}
