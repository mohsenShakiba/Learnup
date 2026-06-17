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
        var existingGrammarTestIds = await dbContext.GrammarTests
            .Select(t => t.GrammarId)
            .ToListAsync(cancellationToken);

        var candidates = await dbContext.Grammars
            .Where(g => !existingGrammarTestIds.Contains(g.Id))
            .Take(5)
            .ToListAsync(cancellationToken);

        foreach (var grammar in candidates)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                var result = await grammarTestProvider.GetGrammarTestAsync(grammar.Name, grammar.Description, cancellationToken);

                var test = new GrammarTest(grammar.Id);
                var options = result.Options.Select(o => new GrammarTestOption(o.Text, o.IsCorrect)).ToList();
                test.Publish(result.Question, options);

                dbContext.GrammarTests.Add(test);
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
