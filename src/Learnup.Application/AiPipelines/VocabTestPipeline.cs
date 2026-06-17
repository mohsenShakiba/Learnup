using System.Diagnostics;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class VocabTestPipeline(
    ILearnupDbContext dbContext,
    IVocabTestProvider vocabTestProvider,
    ILogger<VocabTestPipeline> logger) : IPipeline
{
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var candidates = await dbContext.Vocabs
            .Where(v => v.Status == VocabStatus.Published)
            .Where(v => !v.Tests.Any())
            .Take(5)
            .ToListAsync(cancellationToken);

        foreach (var vocab in candidates)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                var result = await vocabTestProvider.GetVocabTestAsync(vocab,  cancellationToken);

                var test = new VocabTest(vocab.Id);
                var options = result.Options.Select(o => new VocabTestOption(o.Text, o.IsCorrect)).ToList();
                test.Publish(result.Type, result.Question, options);

                dbContext.VocabTests.Add(test);
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "VocabTest for vocab {VocabId} generated in {MilliSeconds}ms",
                    vocab.Id,
                    sw.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error generating test for vocab {VocabId}", vocab.Id);
            }
        }
    }
}