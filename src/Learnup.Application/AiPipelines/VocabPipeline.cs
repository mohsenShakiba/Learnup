using System.Diagnostics;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class VocabPipeline(
    ILearnupDbContext dbContext,
    IVocabTranslationProvider vocabTranslationProvider,
    IVoiceProvider voiceProvider,
    ILogger<VocabPipeline> logger) : IPipeline
{
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var vocabCandidates = await dbContext.Vocabs
            .Where(v => v.Status == VocabStatus.Pending)
            .Take(1)
            .ToListAsync(cancellationToken);

        foreach (var vocab in vocabCandidates)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                var translation = await vocabTranslationProvider.GetTranslationAsync(vocab.Word, cancellationToken);

                vocab.SetTranslation(translation.Translation, translation.Description, translation.ParentWord);

                logger.LogInformation("Vocab {VocabId} was translated in {MilliSeconds}", vocab.Id, sw.ElapsedMilliseconds);

                var transactions = translation.Transactions
                    .Select(transaction => ToVocabTransaction(vocab.Id, transaction))
                    .Where(transaction => transaction is not null)
                    .Select(transaction => transaction!)
                    .ToList();

                if (transactions.Count > 0)
                {
                    dbContext.VocabTransactions.AddRange(transactions);
                }

                logger.LogInformation("Vocab {VocabId} transactions were generated in {MilliSeconds}", vocab.Id, sw.ElapsedMilliseconds);
            }
            catch
            {
                // do nothing
            }

            if (string.IsNullOrWhiteSpace(vocab.VoiceId))
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    var voice = await voiceProvider.GetVoiceAsync(vocab.Word, new VoiceOptions(VoiceIds.Bella, 0.8), cancellationToken);

                    vocab.SetVoice(voice.VoiceId);

                    logger.LogInformation("Vocab {VocabId} voice was generated in {MilliSeconds}", vocab.Id, sw.ElapsedMilliseconds);
                }
                catch
                {
                    // do nothing
                }
            }

            vocab.MarkAsPublished();
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private VocabTransaction? ToVocabTransaction(int vocabId, VocabTransactionResult result)
    {
        if (!Enum.TryParse<VocabTransactionType>(result.Type, true, out var type))
        {
            logger.LogWarning(
                "Skipping vocab {VocabId} transaction with unsupported type {Type}",
                vocabId,
                result.Type);

            return null;
        }

        if (string.IsNullOrWhiteSpace(result.Translation)
            || string.IsNullOrWhiteSpace(result.Example)
            || string.IsNullOrWhiteSpace(result.ExampleTranslation))
        {
            logger.LogWarning("Skipping vocab {VocabId} transaction with missing required fields", vocabId);

            return null;
        }

        return new VocabTransaction(
            vocabId,
            result.Translation,
            type,
            result.Example,
            result.ExampleTranslation,
            result.Description);
    }
}