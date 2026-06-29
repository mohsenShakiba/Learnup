namespace Learnup.Application.ExternalServices;

public record AiProxyMessage(string Role, string Content);

public interface IAiService
{
    Task<T?> SendAsync<T>(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default);
    Task<string> SendAsync(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default);
}