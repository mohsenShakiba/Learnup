namespace Learnup.Application.ExternalServices;

public interface IAiTextService
{
    Task<string> SendAsync(string prompt, CancellationToken cancellationToken = default);
}
