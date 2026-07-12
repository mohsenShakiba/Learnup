using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Learnup.Application.Exceptions;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Application.Prompts;
using Learnup.Domain.AggregateRoots.Chats;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.Services;

public sealed record ChatExecutionOptions(
    bool SaveUserMessageBeforeStreaming,
    bool IgnoreCancellationAfterStart);

public sealed record ChatExecutionResult(
    int ChatId,
    string Reply,
    AiTokenUsage Usage);

public sealed record ChatExecutionUpdate(
    int ChatId,
    string? ContentDelta,
    ChatExecutionResult? Result);

public sealed class ChatExecutionService(
    IAiService aiService,
    ILearnupDbContext dbContext,
    ILogger<ChatExecutionService> logger)
{
    private static readonly TimeSpan LongResponseLogThreshold = TimeSpan.FromSeconds(10);

    public async IAsyncEnumerable<ChatExecutionUpdate> StreamAsync(
        int? chatId,
        int userId,
        string message,
        ChatExecutionOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var totalStopwatch = Stopwatch.StartNew();
        var trimmed = message.Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new ArgumentException("Message is required.", nameof(message));
        }

        var operationCancellationToken = options.IgnoreCancellationAfterStart
            ? CancellationToken.None
            : cancellationToken;

        var chat = await ResolveChatAsync(chatId, userId, cancellationToken);

        var displayName = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);

        var history = chat.Messages.OrderBy(m => m.Id).ToList();
        var proxyMessages = BuildMessages(displayName, history, trimmed);

        chat.AddMessage(ChatRole.User, trimmed);

        if (options.SaveUserMessageBeforeStreaming)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        yield return new ChatExecutionUpdate(chat.Id, null, null);

        var builder = new StringBuilder();
        AiTokenUsage? usage = null;
        var generatedTokenCount = 0;

        await using var enumerator = aiService
            .StreamAsync(proxyMessages, operationCancellationToken)
            .GetAsyncEnumerator(operationCancellationToken);

        while (await MoveNextWithLongWaitLogAsync(
                   enumerator,
                   totalStopwatch,
                   chat.Id,
                   userId,
                   generatedTokenCount,
                   operationCancellationToken))
        {
            var chunk = enumerator.Current;

            if (chunk.Usage is not null)
            {
                usage = chunk.Usage;
            }

            if (string.IsNullOrEmpty(chunk.ContentDelta))
            {
                continue;
            }

            generatedTokenCount++;
            builder.Append(chunk.ContentDelta);

            if (generatedTokenCount == 1)
            {
                logger.LogInformation(
                    "AI chat first token generated for chat {ChatId}, user {UserId} after {ElapsedMilliseconds} ms.",
                    chat.Id,
                    userId,
                    totalStopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.LogDebug(
                    "AI chat token {TokenNumber} generated for chat {ChatId}, user {UserId} after {ElapsedMilliseconds} ms.",
                    generatedTokenCount,
                    chat.Id,
                    userId,
                    totalStopwatch.ElapsedMilliseconds);
            }

            yield return new ChatExecutionUpdate(chat.Id, chunk.ContentDelta, null);
        }

        var reply = builder.ToString();
        var resolvedUsage = usage ?? AiTokenUsage.Empty;

        chat.AddMessage(ChatRole.Assistant, reply, resolvedUsage.CompletionTokens);
        await RecordTokenUsageAsync(dbContext, userId, resolvedUsage, operationCancellationToken);
        await dbContext.SaveChangesAsync(operationCancellationToken);

        logger.LogInformation(
            "AI chat response completed for chat {ChatId}, user {UserId} in {ElapsedMilliseconds} ms with {GeneratedTokenCount} generated tokens.",
            chat.Id,
            userId,
            totalStopwatch.ElapsedMilliseconds,
            generatedTokenCount);

        yield return new ChatExecutionUpdate(chat.Id, null, new ChatExecutionResult(chat.Id, reply, resolvedUsage));
    }

    public static async Task RecordTokenUsageAsync(
        ILearnupDbContext dbContext,
        int userId,
        AiTokenUsage usage,
        CancellationToken cancellationToken)
    {
        var tokenUsage = dbContext.UserTokenUsages.Local
                             .FirstOrDefault(entry => entry.UserId == userId)
                         ?? await dbContext.UserTokenUsages
                             .FirstOrDefaultAsync(entry => entry.UserId == userId, cancellationToken);

        if (tokenUsage is null)
        {
            tokenUsage = new UserTokenUsage(userId);
            dbContext.UserTokenUsages.Add(tokenUsage);
        }

        tokenUsage.Add(usage.PromptTokens, usage.CompletionTokens, DateTime.UtcNow.Date);
    }

    private static List<AiProxyMessage> BuildMessages(
        string? displayName,
        IEnumerable<ChatMessage> history,
        string newUserMessage)
    {
        var messages = new List<AiProxyMessage>
        {
            new("system", ChatPrompt.GetPrompt(displayName))
        };

        foreach (var message in history)
        {
            var role = message.Role == ChatRole.Assistant ? "assistant" : "user";
            messages.Add(new AiProxyMessage(role, message.Content));
        }

        messages.Add(new AiProxyMessage("user", newUserMessage));

        return messages;
    }

    private async Task<Chat> ResolveChatAsync(
        int? chatId,
        int userId,
        CancellationToken cancellationToken)
    {
        if (chatId is null)
        {
            var chat = new Chat(userId);
            dbContext.Chats.Add(chat);
            return chat;
        }

        return await dbContext.Chats
                   .Include(c => c.Messages)
                   .FirstOrDefaultAsync(c => c.Id == chatId && c.UserId == userId, cancellationToken)
               ?? throw new KeyNotFoundException($"Chat {chatId} was not found.");
    }

    private async Task<bool> MoveNextWithLongWaitLogAsync(
        IAsyncEnumerator<AiStreamChunk> enumerator,
        Stopwatch totalStopwatch,
        int chatId,
        int userId,
        int generatedTokenCount,
        CancellationToken cancellationToken)
    {
        var moveNextTask = enumerator.MoveNextAsync().AsTask();

        while (true)
        {
            var delayTask = Task.Delay(LongResponseLogThreshold, cancellationToken);
            var completedTask = await Task.WhenAny(moveNextTask, delayTask);

            if (completedTask == moveNextTask)
            {
                return await moveNextTask;
            }

            await delayTask;

            logger.LogWarning(
                "AI chat response is taking longer than {ElapsedMilliseconds} ms for chat {ChatId}, user {UserId}; generated {GeneratedTokenCount} tokens so far.",
                totalStopwatch.ElapsedMilliseconds,
                chatId,
                userId,
                generatedTokenCount);
        }
    }
}
