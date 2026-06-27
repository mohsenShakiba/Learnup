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
    public bool Enabled => false;
    
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var candidates = await dbContext.LessonVocabs
            .Include(lv => lv.Vocab)
            .Where(lv => lv.Vocab.Status == VocabStatus.Published)
            .Where(lv => !dbContext.Tests.Any(t => t.LessonId == lv.LessonId && t.Type == TestType.Vocab))
            .Take(5)
            .ToListAsync(cancellationToken);

        foreach (var lessonVocab in candidates)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var vocab = lessonVocab.Vocab;

                var result = await vocabTestProvider.GetVocabTestAsync(vocab,  cancellationToken);

                var test = new Test(lessonVocab.LessonId, TestType.Vocab);
                var options = result.Options.Select(o => new TestOption(o.Text, o.IsCorrect)).ToList();
                test.Publish(result.Type, result.Question, options);

                dbContext.Tests.Add(test);
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Test for lesson {LessonId} generated in {MilliSeconds}ms",
                    lessonVocab.LessonId,
                    sw.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error generating test for lesson {LessonId}", lessonVocab.LessonId);
            }
        }
    }
}
