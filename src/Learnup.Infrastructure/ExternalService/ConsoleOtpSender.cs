using Learnup.Application.ExternalServices;
using Microsoft.Extensions.Logging;

namespace Learnup.Infrastructure.ExternalService;

public sealed class ConsoleOtpSender(ILogger<ConsoleOtpSender> logger) : IOtpSender
{
    public Task SendAsync(string mobileNumber, string code, CancellationToken cancellationToken)
    {
        logger.LogInformation("OTP for {MobileNumber}: {Code}", mobileNumber, code);
        return Task.CompletedTask;
    }
}
