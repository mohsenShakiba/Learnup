using System.Diagnostics;
using System.Text;
using Learnup.Application.Authentication;
using Learnup.Application.Exceptions;
using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Learnup.Domain.AggregateRoots.Chats;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.Features.Public.Ai;

public sealed record ChatWithAi(int? ChatId, string Message) : IRequest<ChatResponse>;

internal sealed class ChatWithAiHandler(
    IAiService aiService,
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider,
    ILogger<ChatWithAiHandler> logger)
    : IRequestHandler<ChatWithAi, ChatResponse>
{
    private static readonly TimeSpan LongResponseLogThreshold = TimeSpan.FromSeconds(10);

    public async Task<ChatResponse> Handle(ChatWithAi request, CancellationToken cancellationToken)
    {
        var totalStopwatch = Stopwatch.StartNew();
        var message = request.Message.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message is required.", nameof(request.Message));
        }

        var tokenUsage = await dbContext.UserTokenUsages
            .FirstOrDefaultAsync(entry => entry.UserId == identityProvider.UserId, cancellationToken);

        if (tokenUsage is null)
        {
            tokenUsage = new UserTokenUsage(identityProvider.UserId);
            dbContext.UserTokenUsages.Add(tokenUsage);
        }

        if (tokenUsage.TotalTokens >= tokenUsage.AvailableTokens)
        {
            throw new TokenUsageExceedException();
        }

        var userId = identityProvider.UserId;

        var chat = await ResolveChatAsync(request.ChatId, userId, cancellationToken);

        var displayName = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);

        var history = chat.Messages.OrderBy(m => m.Id).ToList();

        var proxyMessages = ChatSupport.BuildMessages(displayName, history, message);

        chat.AddMessage(ChatRole.User, message);

        var builder = new StringBuilder();
        AiTokenUsage? usage = null;
        var generatedTokenCount = 0;

        await using var enumerator = aiService
            .StreamAsync(proxyMessages, CancellationToken.None)
            .GetAsyncEnumerator(CancellationToken.None);

        while (await MoveNextWithLongWaitLogAsync(enumerator, totalStopwatch, chat.Id, userId, generatedTokenCount))
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
        }

        var content = builder.ToString();
        var resolvedUsage = usage ?? AiTokenUsage.Empty;

        chat.AddMessage(ChatRole.Assistant, content, resolvedUsage.CompletionTokens);

        await ChatSupport.RecordTokenUsageAsync(dbContext, userId, resolvedUsage, CancellationToken.None);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        logger.LogInformation(
            "AI chat response completed for chat {ChatId}, user {UserId} in {ElapsedMilliseconds} ms with {GeneratedTokenCount} generated tokens.",
            chat.Id,
            userId,
            totalStopwatch.ElapsedMilliseconds,
            generatedTokenCount);

        return new ChatResponse(chat.Id, content, resolvedUsage.TotalTokens);
    }

    private async Task<bool> MoveNextWithLongWaitLogAsync(
        IAsyncEnumerator<AiStreamChunk> enumerator,
        Stopwatch totalStopwatch,
        int chatId,
        int userId,
        int generatedTokenCount)
    {
        var moveNextTask = enumerator.MoveNextAsync().AsTask();

        while (true)
        {
            var delayTask = Task.Delay(LongResponseLogThreshold);
            var completedTask = await Task.WhenAny(moveNextTask, delayTask);

            if (completedTask == moveNextTask)
            {
                return await moveNextTask;
            }

            logger.LogWarning(
                "AI chat response is taking longer than {ElapsedMilliseconds} ms for chat {ChatId}, user {UserId}; generated {GeneratedTokenCount} tokens so far.",
                totalStopwatch.ElapsedMilliseconds,
                chatId,
                userId,
                generatedTokenCount);
        }
    }

    private async Task<Chat> ResolveChatAsync(
        int? chatId,
        int userId,
        CancellationToken cancellationToken)
    {
        if (chatId is not int id)
        {
            var chat = new Chat(userId);
            dbContext.Chats.Add(chat);
            return chat;
        }

        return await dbContext.Chats
                   .Include(c => c.Messages)
                   .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken)
               ?? throw new KeyNotFoundException($"Chat {id} was not found.");
    }
}
