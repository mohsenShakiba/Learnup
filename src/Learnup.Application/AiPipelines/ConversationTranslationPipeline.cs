using System.Text.Json;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Application.Prompts;
using Learnup.Domain.AggregateRoots.Conversations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class ConversationTranslationPipeline(
    ILearnupDbContext dbContext,
    IAiService aiService,
    ILogger<ConversationTranslationPipeline> logger) : IPipeline
{
    public bool Enabled => false;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var conversations = await dbContext.Conversations
            .Include(s => s.Items)
            .ThenInclude(i => i.Expressions)
            .Where(s => !s.Status.HasFlag(ConversationStatus.Translated))
            .Where(s => s.Id == 1)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var conversation in conversations)
        {
            try
            {
                await TranslateAsync(conversation, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error translating conversation {ConversationId}", conversation.Id);
            }

            conversation.MarkAsTranslated();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task TranslateAsync(Conversation conversation, CancellationToken cancellationToken)
    {
        var items = conversation.Items.OrderBy(i => i.Order).ToList();

        if (items.Count == 0)
        {
            return;
        }

        var payload = JsonSerializer.Serialize(
            items.Select(i => new { i.Order, i.Content }));

        var result = await aiService.SendAsync<ConversationTranslationResult>(
            [
                new AiProxyMessage("system", ConversationTranslationPrompt.GetPrompt()),
                new AiProxyMessage("user", payload)
            ],
            cancellationToken);

        if (result?.Items is null)
        {
            return;
        }

        var itemsByOrder = items.ToDictionary(i => i.Order);

        foreach (var translated in result.Items)
        {
            if (!itemsByOrder.TryGetValue(translated.Order, out var item))
            {
                continue;
            }

            item.SetTranslation(translated.Translation);

            foreach (var expression in translated.Expressions ?? [])
            {
                item.AddExpression(new ConversationItemExpression(expression.Phrase, expression.Meaning, expression.Translation));
            }
        }
    }

    record ConversationTranslationResult(List<ConversationItemTranslationResult>? Items);

    record ConversationItemTranslationResult(int Order, string Translation, List<ExpressionResult>? Expressions);

    record ExpressionResult(string Phrase, string Meaning, string? Translation);
}
