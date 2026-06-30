using System.Diagnostics;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class VocabVoicePipeline(ILearnupDbContext dbContext, IVoiceProvider voiceProvider, ILogger<VocabVoicePipeline> logger) : IPipeline
{
    public bool Enabled => true;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var vocabs = await dbContext.Vocabs
            .Where(v => v.Status == VocabStatus.Published)
            .Where(vocab => vocab.VoiceId == null)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var vocab in vocabs)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                var result = await voiceProvider.GetVoiceAsync(vocab.Word, new VoiceOptions(VoiceIds.Heart, 0.9),
                    cancellationToken: cancellationToken);

                vocab.SetVoice(result.VoiceId);

                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Vocab voice {Vocab} was processed in {MilliSeconds}ms",
                    vocab.Word,
                    sw.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error processing vocab voice {Vocab}", vocab.Word);
            }
        }
    }
}