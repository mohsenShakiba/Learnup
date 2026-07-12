using Learnup.API.Hubs;
using Learnup.Application.Exceptions;
using Learnup.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Learnup.API.HostedServices;

public sealed class ChatCompletionHostedService(
    IChatCompletionQueue queue,
    IServiceScopeFactory serviceScopeFactory,
    IHubContext<ChatHub> hubContext,
    ILogger<ChatCompletionHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in queue.ReadAllAsync(stoppingToken))
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            await ProcessAsync(job, stoppingToken);
        }
    }

    private async Task ProcessAsync(ChatCompletionJob job, CancellationToken stoppingToken)
    {
        var groupName = ChatHub.UserGroup(job.UserId);
        int? chatId = job.ChatId;

        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var chatExecutionService = scope.ServiceProvider.GetRequiredService<ChatExecutionService>();
            var options = new ChatExecutionOptions(
                SaveUserMessageBeforeStreaming: true,
                IgnoreCancellationAfterStart: false);

            await foreach (var update in chatExecutionService.StreamAsync(
                               job.ChatId,
                               job.UserId,
                               job.Message,
                               options,
                               stoppingToken))
            {
                chatId = update.ChatId;

                if (update is { ContentDelta: null, Result: null })
                {
                    await hubContext.Clients
                        .Group(groupName)
                        .SendAsync(
                            ChatHub.ChatStarted,
                            new
                            {
                                update.ChatId
                            },
                            stoppingToken);
                }

                if (update.ContentDelta is not null)
                {
                    await hubContext.Clients
                        .Group(groupName)
                        .SendAsync(
                            ChatHub.ChatDelta,
                            new
                            {
                                update.ChatId,
                                Delta = update.ContentDelta
                            },
                            stoppingToken);
                }

                if (update.Result is not null)
                {
                    await hubContext.Clients
                        .Group(groupName)
                        .SendAsync(
                            ChatHub.ChatCompleted,
                            new
                            {
                                update.Result.ChatId,
                                update.Result.Reply,
                                TokensUsed = update.Result.Usage.TotalTokens
                            },
                            stoppingToken);
                }
            }
        }
        catch (TokenUsageExceedException)
        {
            await hubContext.Clients
                .Group(groupName)
                .SendAsync(
                    ChatHub.ChatFailed,
                    new
                    {
                        ChatId = chatId,
                        Error = "TokenExceed"
                    },
                    stoppingToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !stoppingToken.IsCancellationRequested)
        {
            logger.LogError(
                exception,
                "Queued AI chat completion failed for chat {ChatId}, user {UserId}.",
                chatId,
                job.UserId);

            await hubContext.Clients
                .Group(groupName)
                .SendAsync(
                    ChatHub.ChatFailed,
                    new
                    {
                        ChatId = chatId,
                        Error = "ChatCompletionFailed"
                    },
                    stoppingToken);
        }
    }
}
