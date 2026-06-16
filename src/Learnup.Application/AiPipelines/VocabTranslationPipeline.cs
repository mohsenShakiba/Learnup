using System.Diagnostics;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class VocabTranslationPipeline(
    ILearnupDbContext dbContext,
    IVocabTranslationProvider vocabTranslationProvider,
    ILogger<VocabTranslationPipeline> logger) : IPipeline
{
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var translations = await dbContext.VocabTransactions
            .Include(translation => translation.Vocab)
            .Where(translation => translation.Status == VocabStatus.Pending)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var translation in translations)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                var result = await vocabTranslationProvider.GetVocabTranslationAsync(
                    translation.Vocab.Word,
                    translation.Type,
                    cancellationToken);

                translation.SetTranslation(
                    result.Translation,
                    result.Description,
                    result.Example,
                    result.ExampleTranslation);

                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Vocab translation {VocabTranslationId} for vocab {VocabId} was processed in {MilliSeconds}",
                    translation.Id,
                    translation.VocabId,
                    sw.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Error processing vocab translation {VocabTranslationId} for vocab {VocabId}",
                    translation.Id,
                    translation.VocabId);
            }
        }
    }
}
