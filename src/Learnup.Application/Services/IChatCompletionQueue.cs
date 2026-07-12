namespace Learnup.Application.Services;

public sealed record ChatCompletionJob(
    int UserId,
    int? ChatId,
    string Message);

public interface IChatCompletionQueue
{
    ValueTask QueueAsync(ChatCompletionJob job, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ChatCompletionJob> ReadAllAsync(CancellationToken cancellationToken = default);
}
