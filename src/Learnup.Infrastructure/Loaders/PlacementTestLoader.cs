using Learnup.Application.Features.Public.Placement;
using Learnup.Application.Requests.Admin.Placement;
using Learnup.Domain.AggregateRoots.Placement;
using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.ExternalService;

public class PlacementTestLoader(LearnupDbContext dbContext)
{
    private static readonly string[] Bands = PlacementScorer.Bands;

    public async Task<int> LoadAsync(PlacementTestRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var existingTests = await dbContext.PlacementTests
            .ToListAsync(cancellationToken);

        if (existingTests.Count > 0)
        {
            dbContext.PlacementTests.RemoveRange(existingTests);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var placementTest = new PlacementTest(
            request.Title.Trim(),
            request.Description.Trim(),
            request.Instructions.Trim());

        foreach (var questionRequest in request.Questions.OrderBy(q => q.Id))
        {
            var question = new PlacementQuestion(
                questionRequest.Id,
                ParseLevel(questionRequest.Level),
                ParseSkill(questionRequest.Skill),
                questionRequest.Prompt.Trim());

            foreach (var optionText in questionRequest.Options)
            {
                var isCorrect = string.Equals(
                    optionText.Trim(),
                    questionRequest.Answer.Trim(),
                    StringComparison.Ordinal);

                question.AddOption(new PlacementOption(optionText.Trim(), isCorrect));
            }

            placementTest.AddQuestion(question);
        }

        dbContext.PlacementTests.Add(placementTest);
        await dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return placementTest.Id;
    }

    private static void Validate(PlacementTestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new InvalidOperationException("Placement test title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new InvalidOperationException("Placement test description is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Instructions))
        {
            throw new InvalidOperationException("Placement test instructions are required.");
        }

        if (request.Questions is null || request.Questions.Count != 24)
        {
            throw new InvalidOperationException("Placement test must contain exactly 24 questions.");
        }

        var ids = request.Questions.Select(q => q.Id).ToList();
        if (ids.Distinct().Count() != ids.Count)
        {
            throw new InvalidOperationException("Placement question ids must be unique.");
        }

        if (ids.OrderBy(id => id).SequenceEqual(Enumerable.Range(1, 24)) is false)
        {
            throw new InvalidOperationException("Placement question ids must be the values 1 through 24.");
        }

        foreach (var band in Bands)
        {
            var countForBand = request.Questions.Count(q => ParseLevel(q.Level) == band);
            if (countForBand != 4)
            {
                throw new InvalidOperationException($"Band '{band}' must contain exactly 4 questions but has {countForBand}.");
            }
        }

        // Ascending by band then by id.
        var ordered = request.Questions
            .OrderBy(q => Array.IndexOf(Bands, ParseLevel(q.Level)))
            .ThenBy(q => q.Id)
            .Select(q => q.Id)
            .ToList();

        if (ordered.SequenceEqual(Enumerable.Range(1, 24)) is false)
        {
            throw new InvalidOperationException("Placement questions must be ordered ascending by band (A1→C2) then by id.");
        }

        foreach (var question in request.Questions)
        {
            _ = ParseSkill(question.Skill);

            if (string.IsNullOrWhiteSpace(question.Prompt))
            {
                throw new InvalidOperationException($"Placement question '{question.Id}' is missing a prompt.");
            }

            if (question.Options is null || question.Options.Count != 4)
            {
                throw new InvalidOperationException($"Placement question '{question.Id}' must have exactly 4 options.");
            }

            var trimmedOptions = question.Options.Select(o => o?.Trim() ?? string.Empty).ToList();

            if (trimmedOptions.Any(string.IsNullOrWhiteSpace))
            {
                throw new InvalidOperationException($"Placement question '{question.Id}' has an empty option.");
            }

            if (trimmedOptions.Distinct(StringComparer.Ordinal).Count() != trimmedOptions.Count)
            {
                throw new InvalidOperationException($"Placement question '{question.Id}' options must be distinct.");
            }

            var matches = trimmedOptions.Count(o => string.Equals(o, question.Answer?.Trim(), StringComparison.Ordinal));
            if (matches != 1)
            {
                throw new InvalidOperationException(
                    $"Placement question '{question.Id}' answer must match exactly one option.");
            }
        }
    }

    private static string ParseLevel(string value)
    {
        var normalized = value?.Trim().ToUpperInvariant();

        if (normalized is not null && Bands.Contains(normalized))
        {
            return normalized;
        }

        throw new FormatException($"Invalid CEFR level '{value}'.");
    }

    private static PlacementSkill ParseSkill(string value)
    {
        if (Enum.TryParse<PlacementSkill>(value?.Trim(), ignoreCase: true, out var skill) && Enum.IsDefined(skill))
        {
            return skill;
        }

        throw new FormatException($"Invalid placement skill '{value}'.");
    }
}
