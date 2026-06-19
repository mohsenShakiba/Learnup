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
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var candidates = await dbContext.Grammars
            .Include(g => g.Lessons)
            .Where(g => g.Id == 10)
            .Where(g => !g.Tests.Any())
            .Take(5)
            .ToListAsync(cancellationToken);

        foreach (var grammar in candidates)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                
                var lessonGrammar = await dbContext.LessonGrammars
                    .Include(g => g.Lesson)
                    .ThenInclude(l => l.Stories)
                    .ThenInclude(s => s.Story)
                    .ThenInclude(s => s.Items)
                    .Where(g => g.GrammarId == grammar.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                
                var story = lessonGrammar?.Lesson.Stories.Select(s => s.Story).FirstOrDefault();

                if (story is null)
                {
                    continue;
                }
                
                var results = await grammarTestProvider.GetGrammarTestAsync(grammar, story, cancellationToken);

                foreach (var result in results)
                {
                    var test = new GrammarTest(grammar.Id);
                    var options = result.Options.Select(o => new GrammarTestOption(o.Text, o.IsCorrect)).ToList();
                    test.Publish(result.Question, options);
                    dbContext.GrammarTests.Add(test);
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "GrammarTest for grammar {GrammarId} generated in {MilliSeconds}ms",
                    grammar.Id,
                    sw.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error generating test for grammar {GrammarId}", grammar.Id);
            }
        }
    }
}