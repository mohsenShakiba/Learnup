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

                var result = await vocabTranslationProvider.GetTranslationAsync(vocab.Word, cancellationToken);

                vocab.SetTranslation(result.Translation, result.Description, result.ParentWord);

                logger.LogInformation("Vocab {VocabId} was translated in {MilliSeconds}", vocab.Id, sw.ElapsedMilliseconds);

                foreach (var typeValue in result.Types)
                {
                    if (!Enum.TryParse<VocabTranslationType>(typeValue, true, out var type))
                    {
                        logger.LogWarning("Skipping vocab {VocabId} transaction with unsupported type {Type}", vocab.Word, typeValue);
                        continue;
                    }
                    
                    var translation = new VocabTranslation(vocab.Id, type);
                    
                    vocab.AddTranslation(translation);
                }
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

}