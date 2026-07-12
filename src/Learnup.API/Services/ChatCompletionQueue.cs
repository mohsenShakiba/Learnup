using System.Threading.Channels;
using Learnup.Application.Services;

namespace Learnup.API.Services;

internal sealed class ChatCompletionQueue : IChatCompletionQueue
{
    private readonly Channel<ChatCompletionJob> _channel = Channel.CreateUnbounded<ChatCompletionJob>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask QueueAsync(ChatCompletionJob job, CancellationToken cancellationToken = default)
    {
        return _channel.Writer.WriteAsync(job, cancellationToken);
    }

    public IAsyncEnumerable<ChatCompletionJob> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
