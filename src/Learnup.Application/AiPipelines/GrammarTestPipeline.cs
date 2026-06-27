using System.Diagnostics;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class GrammarTestPipeline(
    ILearnupDbContext dbContext,
    IGrammarTestProvider grammarTestProvider,
    ILogger<GrammarTestPipeline> logger) : IPipeline
{
    public bool Enabled => false;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var candidates = await dbContext.LessonGrammars
            .Include(lg => lg.Grammar)
            .Where(lg => lg.GrammarId == 10)
            .Where(lg => !dbContext.Tests.Any(t => t.LessonId == lg.LessonId && t.Type == TestType.Grammar))
            .Take(5)
            .ToListAsync(cancellationToken);

        foreach (var lessonGrammarCandidate in candidates)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var grammar = lessonGrammarCandidate.Grammar;
                
                var lessonGrammar = await dbContext.LessonGrammars
                    .Include(g => g.Lesson)
                    .ThenInclude(l => l.Stories)
                    .ThenInclude(s => s.Story)
                    .ThenInclude(s => s.Items)
                    .Where(g => g.LessonId == lessonGrammarCandidate.LessonId && g.GrammarId == grammar.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                
                var story = lessonGrammar?.Lesson.Stories.Select(s => s.Story).FirstOrDefault();

                if (story is null)
                {
                    continue;
                }
                
                var results = await grammarTestProvider.GetGrammarTestAsync(grammar, story, cancellationToken);

                foreach (var result in results)
                {
                    var test = new Test(lessonGrammarCandidate.LessonId, TestType.Grammar);
                    var options = result.Options.Select(o => new TestOption(o.Text, o.IsCorrect)).ToList();
                    test.Publish(result.Type, result.Question, options);
                    dbContext.Tests.Add(test);
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Test for lesson {LessonId} generated in {MilliSeconds}ms",
                    lessonGrammarCandidate.LessonId,
                    sw.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error generating test for lesson {LessonId}", lessonGrammarCandidate.LessonId);
            }
        }
    }
}
