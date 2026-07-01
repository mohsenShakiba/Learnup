using System.Text;
using Learnup.API.Requests;
using Learnup.API.Responses;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Requests.Admin.Placement;
using Learnup.Application.Requests.Admin.Stories;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.ExternalService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learnup.API.Areas.Admin.Controllers;

public class ImportController(
    VocabLoader vocabLoader,
    StoryLoader storyLoader,
    GrammarLoader grammarLoader,
    PlacementTestLoader placementTestLoader,
    ILearnupDbContext dbContext) : BaseAdminController
{
    [HttpPost("vocabs", Name = "ImportVocabs")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImportVocabsResponse>> ImportVocabs(
        [FromForm] ImportVocabsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File.Length == 0)
        {
            return BadRequest("CSV file is empty.");
        }

        if (!Path.GetExtension(request.File.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only CSV files are supported.");
        }

        await using var stream = request.File.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        List<VocabImportItem> vocabs;

        try
        {
            vocabs = await ReadVocabsAsync(reader, cancellationToken);
        }
        catch (FormatException exception)
        {
            return BadRequest(exception.Message);
        }

        if (vocabs.Count == 0)
        {
            return BadRequest("CSV file does not contain any vocab words.");
        }

        var importedCount = await vocabLoader.LoadAsync(
            vocabs,
            request.LevelId,
            request.LanguageId,
            cancellationToken);

        return Ok(new ImportVocabsResponse(vocabs.Count, importedCount));
    }

    [HttpPost("stories/{courseId:int}/{lessonOrder:int}", Name = "ImportStory")]
    public async Task<ActionResult<int>> ImportStory(
        int courseId,
        int lessonOrder,
        [FromBody] StoryRequest request,
        CancellationToken cancellationToken)
    {
        var storyId = await storyLoader.LoadAsync(
            request,
            courseId,
            lessonOrder,
            cancellationToken);

        return Ok(storyId);
    }

    [HttpPost("placement-test", Name = "ImportPlacementTest")]
    public async Task<ActionResult<int>> ImportPlacementTest(
        [FromBody] PlacementTestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var placementTestId = await placementTestLoader.LoadAsync(request, cancellationToken);
            return Ok(new { placementTestId });
        }
        catch (Exception exception) when (exception is InvalidOperationException or FormatException)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("grammars")]
    public async Task<ActionResult<int>> ImportGrammar(
        [FromBody] ImportGrammarRequest request,
        CancellationToken cancellationToken)
    {
        var grammarId = await grammarLoader.LoadAsync(
            request.Grammar,
            cancellationToken);

        return Ok(grammarId);
    }

    [HttpPost("lesson-grammars", Name = "ImportLessonGrammars")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImportLessonGrammarsResponse>> ImportLessonGrammars(
        [FromForm] ImportLessonGrammarsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File.Length == 0)
        {
            return BadRequest("CSV file is empty.");
        }

        if (!Path.GetExtension(request.File.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only CSV files are supported.");
        }

        await using var stream = request.File.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        List<LessonGrammarImportItem> mappings;

        try
        {
            mappings = await ReadLessonGrammarsAsync(reader, cancellationToken);
        }
        catch (FormatException exception)
        {
            return BadRequest(exception.Message);
        }

        if (mappings.Count == 0)
        {
            return BadRequest("CSV file does not contain any lesson grammar mappings.");
        }

        try
        {
            var importedCount = await ImportLessonGrammarMappingsAsync(mappings, cancellationToken);
            return Ok(new ImportLessonGrammarsResponse(mappings.Count, importedCount));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    private static async Task<List<VocabImportItem>> ReadVocabsAsync(
        TextReader reader,
        CancellationToken cancellationToken)
    {
        var vocabs = new List<VocabImportItem>();
        Dictionary<string, int>? headerIndexes = null;
        var isFirstRow = true;
        var rowNumber = 0;

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            rowNumber++;
            var columns = ParseCsvLine(line);
            if (columns.Count == 0 || columns.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            if (isFirstRow && IsHeaderRow(columns))
            {
                headerIndexes = BuildHeaderIndexes(columns);
                isFirstRow = false;
                continue;
            }

            isFirstRow = false;

            var word = GetColumn(columns, headerIndexes, "word", 0);
            if (string.IsNullOrWhiteSpace(word))
            {
                continue;
            }

            var translation = RequireColumn(columns, headerIndexes, "translation", 1, rowNumber);

            var level = ParseOptionalEnum<VocabLevel>(GetColumn(columns, headerIndexes, "level", 6), "level", rowNumber);

            vocabs.Add(new VocabImportItem(
                word,
                translation,
                level));
        }

        return vocabs;
    }

    private async Task<int> ImportLessonGrammarMappingsAsync(
        IReadOnlyCollection<LessonGrammarImportItem> mappings,
        CancellationToken cancellationToken)
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

    private static async Task<List<LessonGrammarImportItem>> ReadLessonGrammarsAsync(
        TextReader reader,
        CancellationToken cancellationToken)
    {
        var mappings = new List<LessonGrammarImportItem>();
        Dictionary<string, int>? headerIndexes = null;
        var isFirstRow = true;
        var rowNumber = 0;

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            rowNumber++;
            var columns = ParseCsvLine(line);
            if (columns.Count == 0 || columns.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            if (isFirstRow && IsLessonGrammarHeaderRow(columns))
            {
                headerIndexes = BuildLessonGrammarHeaderIndexes(columns);
                isFirstRow = false;
                continue;
            }

            isFirstRow = false;

            mappings.Add(new LessonGrammarImportItem(
                ParseInt(RequireColumn(columns, headerIndexes, "level", 0, rowNumber), "level", rowNumber),
                ParseInt(RequireColumn(columns, headerIndexes, "lessonOrder", 1, rowNumber), "lesson_order", rowNumber),
                ParseEnum<VocabLevel>(
                    RequireColumn(columns, headerIndexes, "grammarLevel", 2, rowNumber),
                    "grammar_level",
                    rowNumber),
                ParseInt(RequireColumn(columns, headerIndexes, "grammarOrder", 3, rowNumber), "grammar_order", rowNumber)));
        }

        return mappings;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var columns = new List<string>();
        var value = new StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var current = line[index];

            if (current == '"')
            {
                if (inQuotes && index + 1 < line.Length && line[index + 1] == '"')
                {
                    value.Append('"');
                    index++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (current == ',' && !inQuotes)
            {
                columns.Add(value.ToString());
                value.Clear();
                continue;
            }

            value.Append(current);
        }

        columns.Add(value.ToString());
        return columns;
    }

    private static bool IsHeaderRow(IReadOnlyList<string> columns)
    {
        var headers = columns
            .Select(NormalizeHeader)
            .ToList();

        return headers.FirstOrDefault() == "word" ||
               headers.Count(header => header is "translation" or "type" or "typeid" or "vocabtype") >= 2;
    }

    private static bool IsLessonGrammarHeaderRow(IReadOnlyList<string> columns)
    {
        var headers = columns
            .Select(NormalizeHeader)
            .ToList();

        return headers.FirstOrDefault() == "level" &&
               headers.Contains("lessonorder") &&
               headers.Contains("grammarlevel") &&
               headers.Contains("grammarorder");
    }

    private static Dictionary<string, int> BuildHeaderIndexes(IReadOnlyList<string> columns)
    {
        var indexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < columns.Count; index++)
        {
            indexes[NormalizeHeader(columns[index])] = index;
        }

        AddAlias(indexes, "typeid", "type");
        AddAlias(indexes, "vocabtype", "type");
        AddAlias(indexes, "levelid", "level");
        AddAlias(indexes, "vocablevel", "level");
        AddAlias(indexes, "exampletranslation", "exampleTranslation");
        AddAlias(indexes, "voiceref", "voiceId");

        return indexes;
    }

    private static Dictionary<string, int> BuildLessonGrammarHeaderIndexes(IReadOnlyList<string> columns)
    {
        var indexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < columns.Count; index++)
        {
            indexes[NormalizeHeader(columns[index])] = index;
        }

        return indexes;
    }

    private static void AddAlias(Dictionary<string, int> indexes, string alias, string target)
    {
        if (indexes.TryGetValue(alias, out var index) && !indexes.ContainsKey(target))
        {
            indexes[target] = index;
        }
    }

    private static string? GetColumn(
        IReadOnlyList<string> columns,
        IReadOnlyDictionary<string, int>? headerIndexes,
        string name,
        int fallbackIndex)
    {
        var index = headerIndexes is not null && headerIndexes.TryGetValue(name, out var headerIndex)
            ? headerIndex
            : fallbackIndex;

        return index < columns.Count
            ? columns[index].Trim()
            : null;
    }

    private static string RequireColumn(
        IReadOnlyList<string> columns,
        IReadOnlyDictionary<string, int>? headerIndexes,
        string name,
        int fallbackIndex,
        int rowNumber)
    {
        var value = GetColumn(columns, headerIndexes, name, fallbackIndex);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new FormatException($"CSV row {rowNumber} is missing required '{name}' value.");
        }

        return value;
    }

    private static int ParseInt(string value, string columnName, int rowNumber)
    {
        if (int.TryParse(value, out var parsed))
        {
            return parsed;
        }

        throw new FormatException($"CSV row {rowNumber} has invalid '{columnName}' value '{value}'.");
    }

    private static TEnum ParseEnum<TEnum>(string value, string columnName, int rowNumber)
        where TEnum : struct, Enum
    {
        if (int.TryParse(value, out var enumValue) && Enum.IsDefined(typeof(TEnum), enumValue))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), enumValue);
        }

        if (Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
        {
            return parsed;
        }

        throw new FormatException($"CSV row {rowNumber} has invalid '{columnName}' value '{value}'.");
    }

    private static TEnum? ParseOptionalEnum<TEnum>(string? value, string columnName, int rowNumber)
        where TEnum : struct, Enum
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : ParseEnum<TEnum>(value, columnName, rowNumber);
    }

    private static string NormalizeHeader(string value)
    {
        return value
            .Trim()
            .Replace("_", string.Empty)
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty)
            .ToLowerInvariant();
    }

    private sealed record LessonGrammarImportItem(
        int CourseId,
        int LessonOrder,
        VocabLevel GrammarLevel,
        int GrammarOrder);
}
