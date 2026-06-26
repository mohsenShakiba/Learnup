namespace Learnup.Application.ExternalServices;

public interface IOtpSender
{
    Task SendAsync(string mobileNumber, string code, CancellationToken cancellationToken);
}
