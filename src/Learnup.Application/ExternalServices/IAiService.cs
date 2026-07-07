namespace Learnup.Application.ExternalServices;

public record AiProxyMessage(string Role, string Content);

public record AiTokenUsage(int PromptTokens, int CompletionTokens, int TotalTokens)
{
    public static readonly AiTokenUsage Empty = new(0, 0, 0);
}

public record AiCompletion(string Content, AiTokenUsage Usage);

/// <summary>
/// A single chunk of a streamed chat completion. <see cref="ContentDelta"/> carries incremental text;
/// <see cref="Usage"/> is populated only on the final chunk (when the provider reports usage).
/// </summary>
public record AiStreamChunk(string? ContentDelta, AiTokenUsage? Usage);

public interface IAiService
{
    Task<T?> SendAsync<T>(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default);
    Task<string> SendAsync(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a chat completion request and returns the raw content along with the token usage reported by the provider.
    /// </summary>
    Task<AiCompletion> CompleteAsync(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams a chat completion, yielding incremental content deltas as they arrive from the provider.
    /// </summary>
    IAsyncEnumerable<AiStreamChunk> StreamAsync(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default);
}