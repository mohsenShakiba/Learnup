using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.ExternalService;

public sealed record LessonGrammarImportItem(
    int CourseId,
    int LessonOrder,
    VocabLevel GrammarLevel,
    int GrammarOrder);

public class LessonGrammarLoader(LearnupDbContext dbContext)
{
    public async Task<int> LoadCsvAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        CsvImportReader.EnsureCsvFile(stream, fileName);

        using var reader = CsvImportReader.CreateReader(stream);
        var mappings = await ReadLessonGrammarsAsync(reader, cancellationToken);

        if (mappings.Count == 0)
        {
            throw new FormatException("CSV file does not contain any lesson grammar mappings.");
        }

        return await LoadAsync(mappings, cancellationToken);
    }

    public async Task<int> LoadAsync(
        IReadOnlyCollection<LessonGrammarImportItem> mappings,
        CancellationToken cancellationToken = default)
    {
        var distinctMappings = mappings
            .GroupBy(mapping => new
            {
                mapping.CourseId,
                mapping.LessonOrder,
                mapping.GrammarLevel,
                mapping.GrammarOrder
            })
            .Select(group => group.First())
            .ToList();

        var courseIds = distinctMappings
            .Select(mapping => mapping.CourseId)
            .Distinct()
            .ToList();
        var lessonOrders = distinctMappings
            .Select(mapping => mapping.LessonOrder)
            .Distinct()
            .ToList();
        var grammarLevels = distinctMappings
            .Select(mapping => mapping.GrammarLevel)
            .Distinct()
            .ToList();
        var grammarOrders = distinctMappings
            .Select(mapping => mapping.GrammarOrder)
            .Distinct()
            .ToList();

        var lessons = await dbContext.Lessons
            .Where(lesson => courseIds.Contains(lesson.CourseId) && lessonOrders.Contains(lesson.Order))
            .ToListAsync(cancellationToken);
        var duplicateLesson = lessons
            .GroupBy(lesson => new { lesson.CourseId, lesson.Order })
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateLesson is not null)
        {
            throw new InvalidOperationException(
                $"Multiple lessons were found with course id '{duplicateLesson.Key.CourseId}' and order '{duplicateLesson.Key.Order}'.");
        }

        var lessonMap = lessons.ToDictionary(lesson => (lesson.CourseId, lesson.Order), lesson => lesson.Id);

        var grammars = await dbContext.Grammars
            .Where(grammar => grammarLevels.Contains(grammar.Level) && grammarOrders.Contains(grammar.Order))
            .ToListAsync(cancellationToken);
        var duplicateGrammar = grammars
            .GroupBy(grammar => new { grammar.Level, grammar.Order })
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateGrammar is not null)
        {
            throw new InvalidOperationException(
                $"Multiple grammars were found with level '{(int)duplicateGrammar.Key.Level}' and order '{duplicateGrammar.Key.Order}'.");
        }

        var grammarMap = grammars.ToDictionary(grammar => (grammar.Level, grammar.Order), grammar => grammar.Id);

        var lessonGrammarIds = new List<(int LessonId, int GrammarId)>();

        foreach (var mapping in distinctMappings)
        {
            if (!lessonMap.TryGetValue((mapping.CourseId, mapping.LessonOrder), out var lessonId))
            {
                throw new InvalidOperationException(
                    $"Lesson with course id '{mapping.CourseId}' and order '{mapping.LessonOrder}' was not found.");
            }

            if (!grammarMap.TryGetValue((mapping.GrammarLevel, mapping.GrammarOrder), out var grammarId))
            {
                throw new InvalidOperationException(
                    $"Grammar with level '{(int)mapping.GrammarLevel}' and order '{mapping.GrammarOrder}' was not found.");
            }

            lessonGrammarIds.Add((lessonId, grammarId));
        }

        var lessonIds = lessonGrammarIds
            .Select(mapping => mapping.LessonId)
            .Distinct()
            .ToList();
        var grammarIds = lessonGrammarIds
            .Select(mapping => mapping.GrammarId)
            .Distinct()
            .ToList();

        var existingMappings = await dbContext.LessonGrammars
            .Where(mapping => lessonIds.Contains(mapping.LessonId) && grammarIds.Contains(mapping.GrammarId))
            .Select(mapping => new { mapping.LessonId, mapping.GrammarId })
            .ToListAsync(cancellationToken);
        var existingMappingSet = existingMappings
            .Select(mapping => (mapping.LessonId, mapping.GrammarId))
            .ToHashSet();

        var newMappings = lessonGrammarIds
            .Distinct()
            .Where(mapping => !existingMappingSet.Contains(mapping))
            .Select(mapping => new LessonGrammar(mapping.LessonId, mapping.GrammarId))
            .ToList();

        if (newMappings.Count == 0)
        {
            return 0;
        }

        dbContext.LessonGrammars.AddRange(newMappings);
        await dbContext.SaveChangesAsync(cancellationToken);

        return newMappings.Count;
    }

    private static Task<List<LessonGrammarImportItem>> ReadLessonGrammarsAsync(
        TextReader reader,
        CancellationToken cancellationToken)
    {
        return CsvImportReader.ReadAsync(
            reader,
            IsHeaderRow,
            columns => CsvImportReader.BuildHeaderIndexes(columns),
            MapRow,
            cancellationToken);
    }

    private static LessonGrammarImportItem MapRow(
        IReadOnlyList<string> columns,
        IReadOnlyDictionary<string, int>? headerIndexes,
        int rowNumber)
    {
        return new LessonGrammarImportItem(
            CsvImportReader.ParseInt(
                CsvImportReader.RequireColumn(columns, headerIndexes, "level", 0, rowNumber),
                "level",
                rowNumber),
            CsvImportReader.ParseInt(
                CsvImportReader.RequireColumn(columns, headerIndexes, "lessonOrder", 1, rowNumber),
                "lesson_order",
                rowNumber),
            CsvImportReader.ParseEnum<VocabLevel>(
                CsvImportReader.RequireColumn(columns, headerIndexes, "grammarLevel", 2, rowNumber),
                "grammar_level",
                rowNumber),
            CsvImportReader.ParseInt(
                CsvImportReader.RequireColumn(columns, headerIndexes, "grammarOrder", 3, rowNumber),
                "grammar_order",
                rowNumber));
    }

    private static bool IsHeaderRow(IReadOnlyList<string> columns)
    {
        var headers = columns
            .Select(CsvImportReader.NormalizeHeader)
            .ToList();

        return headers.FirstOrDefault() == "level" &&
               headers.Contains("lessonorder") &&
               headers.Contains("grammarlevel") &&
               headers.Contains("grammarorder");
    }
}
