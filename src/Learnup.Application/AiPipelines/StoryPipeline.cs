using System.Text.Json;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Application.Prompts;
using Learnup.Domain.AggregateRoots.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class StoryPipeline(
    ILearnupDbContext dbContext,
    IVoiceProvider voiceProvider,
    IAiService aiService,
    ILogger<StoryPipeline> logger) : IPipeline
{
    public bool Enabled => true;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var stories = await dbContext.Stories
            .Where(s => s.Id == 231)
            .Include(s => s.Items)
            .ThenInclude(i => i.Expressions)
            .Where(s => s.Status == StoryStatus.Pending)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var story in stories)
        {
            foreach (var item in story.Items)
            {
                try
                {
                    var option = item.Person == 1 ? new VoiceOptions(VoiceIds.Heart, 0.8) : new VoiceOptions(VoiceIds.Bella, 0.8);
                    var result = await voiceProvider.GetVoiceAsync(item.Content, option, cancellationToken: cancellationToken);
                    item.SetVoice(result.VoiceId);
                }
                catch
                {
                    // do nothing
                }
            }

            try
            {
                await TranslateAsync(story, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error translating story {StoryId}", story.Id);
            }

            story.MarkAsCompleted();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task TranslateAsync(Story story, CancellationToken cancellationToken)
    {
        var items = story.Items.OrderBy(i => i.Order).ToList();

        if (items.Count == 0)
        {
            return;
        }

        var payload = JsonSerializer.Serialize(
            items.Select(i => new { i.Order, i.Content }));

        var result = await aiService.SendAsync<StoryTranslationResult>(
            [
                new AiProxyMessage("system", StoryTranslationPrompt.GetPrompt()),
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
                item.AddExpression(new StoryItemExpression(expression.Phrase, expression.Meaning, expression.Translation));
            }
        }
    }

    record StoryTranslationResult(List<StoryItemTranslationResult>? Items);

    record StoryItemTranslationResult(int Order, string Translation, List<ExpressionResult>? Expressions);

    record ExpressionResult(string Phrase, string Meaning, string? Translation);
}
