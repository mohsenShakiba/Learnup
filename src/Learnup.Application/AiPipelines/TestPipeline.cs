using System.Diagnostics;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Prompts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class TestPipeline(
    ILearnupDbContext dbContext,
    IAiService aiService,
    ILogger<TestPipeline> logger) : IPipeline
{
    public bool Enabled => true;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var targetLesson = await dbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == 40, cancellationToken: cancellationToken);

        if (targetLesson is null)
        {
            return;
        }

        var previousLessons = await dbContext.Lessons
            .Include(l => l.Grammars).ThenInclude(g => g.Grammar)
            .Include(l => l.Vocabs).ThenInclude(lv => lv.Vocab)
            .Where(l => l.Order < targetLesson.Order && l.CourseId <= targetLesson.CourseId)
            .ToListAsync(cancellationToken);

        try
        {
            var sw = Stopwatch.StartNew();

            var grammars = previousLessons
                .SelectMany(l => l.Grammars)
                .Select(g => g.Grammar)
                .Select(g => g.Name)
                .OrderBy(v => Guid.NewGuid())
                .Take(10)
                .ToList();

            var vocabs = previousLessons
                .SelectMany(l => l.Vocabs)
                .Select(g => g.Vocab)
                .Select(g => $"{g.Word} {g.Translation}")
                .OrderBy(v => Guid.NewGuid())
                .Take(20)
                .ToList();

            var userMessage = $"""
                               Words: {string.Join(", ", vocabs)}
                               Grammars: {string.Join(", ", grammars)}
                               """;

            var generatedTests =  await aiService.SendAsync<TestGenerationResult[]>(
                [
                    new AiProxyMessage("system", VocabTestPrompt.GetPrompt()),
                    new AiProxyMessage("user", userMessage)
                ],
                cancellationToken) ?? [];

            foreach (var generatedTest in generatedTests)
            {
                var test = new Test(targetLesson.Id, TestType.Vocab);
                var options = generatedTest.Options.Select(o => new TestOption(o.Text, o.IsCorrect)).ToList();
                test.Publish(generatedTest.Type, generatedTest.Question, options);
                dbContext.Tests.Add(test);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Test for lesson {LessonId} generated in {MilliSeconds}ms",
                targetLesson.Id,
                sw.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error generating test for lesson {LessonId}", targetLesson.Id);
        }
    }
    
    record TestGenerationResult(TestQuestionType Type, string Question, TestOptionResult[] Options);

    record TestOptionResult(string Text, bool IsCorrect);
}