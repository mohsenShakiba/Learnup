using System.Diagnostics;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class VocalPipeline(
    ILearnupDbContext dbContext,
    IVocabTranslationProvider vocabTranslationProvider,
    ILogger<VocalPipeline> logger) : IPipeline
{
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        return;
        
        var vocabs = await dbContext.Vocabs
            .Where(v => string.IsNullOrWhiteSpace(v.Translation))
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var vocab in vocabs)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                var translation = await vocabTranslationProvider.GetTranslationAsync(vocab.Word, cancellationToken);

                int? parentId = null;

                if (translation.ParentWord != null)
                {
                    parentId = await dbContext.Vocabs
                        .Where(v => v.Word == translation.ParentWord)
                        .Select(v => v.Id)
                        .SingleOrDefaultAsync(cancellationToken);
                }

                vocab.SetTranslation(translation.Translation, translation.Description, parentId);

                logger.LogInformation("Vocab {VocabId} was translated in {MilliSeconds}", vocab.Id, sw.ElapsedMilliseconds);
            }
            catch
            {
                // do nothing
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}