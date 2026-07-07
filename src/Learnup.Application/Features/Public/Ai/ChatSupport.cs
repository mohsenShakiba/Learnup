using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Application.Prompts;
using Learnup.Domain.AggregateRoots.Conversations;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

/// <summary>
/// Shared helpers for the chat features: building the provider message list from
/// conversation history and accumulating per-user token usage.
/// </summary>
internal static class ChatSupport
{
    public static List<AiProxyMessage> BuildMessages(
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

    /// <summary>
    /// Adds the reported token usage to the user's running total. Does not save; the caller
    /// is expected to call <see cref="ILearnupDbContext.SaveChangesAsync"/>.
    /// </summary>
    public static async Task RecordTokenUsageAsync(
        ILearnupDbContext dbContext,
        int userId,
        AiTokenUsage usage,
        CancellationToken cancellationToken)
    {
        var tokenUsage = await dbContext.UserTokenUsages
            .FirstOrDefaultAsync(entry => entry.UserId == userId, cancellationToken);

        if (tokenUsage is null)
        {
            tokenUsage = new UserTokenUsage(userId);
            dbContext.UserTokenUsages.Add(tokenUsage);
        }

        tokenUsage.Add(usage.PromptTokens, usage.CompletionTokens);
    }
}
