namespace Learnup.Application.Features.Public.Ai;

/// <summary>
/// Streams an AI reply for a conversation, persisting both the user's message and the
/// assistant's reply and accumulating token usage. Transport-agnostic so it can be driven
/// from a SignalR hub (or any other streaming surface).
/// </summary>
public interface IChatStreamService
{
    IAsyncEnumerable<string> StreamAsync(
        int userId,
        int conversationId,
        string message,
        CancellationToken cancellationToken = default);
}
