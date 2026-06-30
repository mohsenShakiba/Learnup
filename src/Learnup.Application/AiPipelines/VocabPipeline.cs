using System.Diagnostics;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Application.Prompts;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Prompts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class VocabPipeline(
    ILearnupDbContext dbContext,
    IAiService aiService,
    ILogger<VocabPipeline> logger) : IPipeline
{
    public bool Enabled => false;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var vocabs = await dbContext.Vocabs
            .Where(translation => translation.Status == VocabStatus.Pending)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var vocab in vocabs)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                var translation = await aiService.SendAsync<TranslationResult>(
                    [
                        new AiProxyMessage("system", VocabTranslationPrompt.GetPrompt()),
                        new AiProxyMessage("user", vocab.Word)
                    ],
                    cancellationToken);

                if (translation == null)
                {
                    continue;
                }

                vocab.SetTranslation(translation.Translation, translation.Description);

                foreach (var type in translation.Types)
                {
                    var typeTranslation = new VocabSense(vocab.Id, type.Translation, type.Description, type.Example,
                        type.ExampleTranslation, type.Type);

                    vocab.AddType(typeTranslation);
                }
                
                vocab.MarkAsPublished();

                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Vocab translation {Vocab} was processed in {MilliSeconds}",
                    vocab.Word,
                    sw.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error processing vocab {Vocab}", vocab.Word);
            }
        }
    }

    record TranslationResult(string Translation, string? Description, List<TypeTranslationResult> Types);

    record TypeTranslationResult(string Translation, string? Description, string Example, string ExampleTranslation, VocabType Type);
}