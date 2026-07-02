using System.Text;

namespace Learnup.Infrastructure.ExternalService;

internal static class CsvImportReader
{
    public static async Task<List<TItem>> ReadAsync<TItem>(
        TextReader reader,
        Func<IReadOnlyList<string>, bool> isHeaderRow,
        Func<IReadOnlyList<string>, Dictionary<string, int>> buildHeaderIndexes,
        Func<IReadOnlyList<string>, IReadOnlyDictionary<string, int>?, int, TItem?> mapRow,
        CancellationToken cancellationToken)
    {
        var items = new List<TItem>();
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

            if (isFirstRow && isHeaderRow(columns))
            {
                headerIndexes = buildHeaderIndexes(columns);
                isFirstRow = false;
                continue;
            }

            isFirstRow = false;

            var item = mapRow(columns, headerIndexes, rowNumber);
            if (item is not null)
            {
                items.Add(item);
            }
        }

        return items;
    }

    public static StreamReader CreateReader(Stream stream)
    {
        return new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
    }

    public static void EnsureCsvFile(Stream stream, string fileName)
    {
        if (stream.CanSeek && stream.Length == 0)
        {
            throw new FormatException("CSV file is empty.");
        }

        if (!Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            throw new FormatException("Only CSV files are supported.");
        }
    }

    public static string? GetColumn(
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

    public static string RequireColumn(
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

    public static int ParseInt(string value, string columnName, int rowNumber)
    {
        if (int.TryParse(value, out var parsed))
        {
            return parsed;
        }

        throw new FormatException($"CSV row {rowNumber} has invalid '{columnName}' value '{value}'.");
    }

    public static TEnum ParseEnum<TEnum>(string value, string columnName, int rowNumber)
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

    public static TEnum? ParseOptionalEnum<TEnum>(string? value, string columnName, int rowNumber)
        where TEnum : struct, Enum
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : ParseEnum<TEnum>(value, columnName, rowNumber);
    }

    public static Dictionary<string, int> BuildHeaderIndexes(
        IReadOnlyList<string> columns,
        IReadOnlyDictionary<string, string>? aliases = null)
    {
        var indexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < columns.Count; index++)
        {
            indexes[NormalizeHeader(columns[index])] = index;
        }

        if (aliases is null)
        {
            return indexes;
        }

        foreach (var alias in aliases)
        {
            if (indexes.TryGetValue(alias.Key, out var index) && !indexes.ContainsKey(alias.Value))
            {
                indexes[alias.Value] = index;
            }
        }

        return indexes;
    }

    public static string NormalizeHeader(string value)
    {
        return value
            .Trim()
            .Replace("_", string.Empty)
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty)
            .ToLowerInvariant();
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
}
