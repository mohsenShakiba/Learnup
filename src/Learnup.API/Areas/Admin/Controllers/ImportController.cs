using System.Text;
using Learnup.API.Requests;
using Learnup.API.Responses;
using Learnup.Application.Mediation;
using Learnup.Application.Requests.Admin.Stories;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.ExternalService;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Admin.Controllers;


public class ImportController(
    VocabLoader vocabLoader,
    StoryLoader storyLoader,
    GrammarLoader grammarLoader) : BaseAdminController
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
        [FromBody] StoryRequest  request,
        CancellationToken cancellationToken)
    {
        var storyId = await storyLoader.LoadAsync(
            request,
            courseId,
            lessonOrder,
            cancellationToken);

        return Ok(storyId);
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
            var type = ParseEnum<VocabType>(
                RequireColumn(columns, headerIndexes, "type", 2, rowNumber),
                "type",
                rowNumber);
            var level = ParseOptionalEnum<VocabLevel>(GetColumn(columns, headerIndexes, "level", 6), "level", rowNumber);

            vocabs.Add(new VocabImportItem(
                word,
                translation,
                type,
                level,
                GetColumn(columns, headerIndexes, "description", 3),
                GetColumn(columns, headerIndexes, "example", 4),
                GetColumn(columns, headerIndexes, "exampleTranslation", 5),
                GetColumn(columns, headerIndexes, "voiceId", 7)));
        }

        return vocabs;
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
}
