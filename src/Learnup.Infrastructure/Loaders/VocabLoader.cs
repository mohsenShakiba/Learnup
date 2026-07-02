using Learnup.Application.ExternalServices;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.ExternalService;

public sealed record VocabImportItem(
    string Word,
    string? Translation,
    VocabLevel? Level = null,
    string? Description = null);

public class VocabLoader(LearnupDbContext dbContext)
{
    public async Task<int> LoadCsvAsync(
        Stream stream,
        string fileName,
        int defaultLevelId,
        int languageId,
        CancellationToken cancellationToken = default)
    {
        CsvImportReader.EnsureCsvFile(stream, fileName);

        using var reader = CsvImportReader.CreateReader(stream);
        var vocabs = await ReadVocabsAsync(reader, cancellationToken);

        if (vocabs.Count == 0)
        {
            throw new FormatException("CSV file does not contain any vocab words.");
        }

        return await LoadAsync(vocabs, defaultLevelId, languageId, cancellationToken);
    }

    public async Task<int> LoadAsync(
        IReadOnlyCollection<VocabImportItem> vocabs,
        int defaultLevelId,
        int languageId,
        CancellationToken cancellationToken = default)
    {
        if (vocabs.Count == 0)
        {
            return 0;
        }

        if (!Enum.IsDefined(typeof(VocabLevel), defaultLevelId))
        {
            throw new ArgumentOutOfRangeException(nameof(defaultLevelId), defaultLevelId, "Unknown vocab level id.");
        }

        var languageExists = await dbContext.Languages
            .AnyAsync(language => language.Id == languageId, cancellationToken);

        if (!languageExists)
        {
            throw new InvalidOperationException($"Language with id '{languageId}' was not found.");
        }

        var defaultLevel = (VocabLevel)defaultLevelId;
        var normalizedVocabs = vocabs
            .Select(vocab => vocab with
            {
                Word = vocab.Word.Trim(),
                Translation = NormalizeOptional(vocab.Translation),
                Description = NormalizeOptional(vocab.Description),
                Level = vocab.Level ?? defaultLevel
            })
            .Where(vocab => !string.IsNullOrWhiteSpace(vocab.Word))
            .GroupBy(vocab => new
            {
                Word = vocab.Word.ToLower(),
                Level = vocab.Level!.Value
            })
            .Select(group => group.First())
            .ToList();

        if (normalizedVocabs.Count == 0)
        {
            return 0;
        }

        var levels = normalizedVocabs
            .Select(vocab => vocab.Level!.Value)
            .Distinct()
            .ToList();

        var lowerWords = normalizedVocabs
            .Select(vocab => vocab.Word.ToLower())
            .Distinct()
            .ToList();

        var existingVocabs = await dbContext.Vocabs
            .Where(vocab =>
                vocab.LanguageId == languageId &&
                levels.Contains(vocab.Level) &&
                lowerWords.Contains(vocab.Word.ToLower()))
            .ToListAsync(cancellationToken);

        var existingVocabMap = existingVocabs
            .ToDictionary(
                vocab => vocab.Word.ToLower(),
                vocab => vocab);

        var updatedCount = 0;

        foreach (var vocab in normalizedVocabs)
        {
            if (!existingVocabMap.TryGetValue(vocab.Word.ToLower(), out var existingVocab))
            {
                continue;
            }

            existingVocab.SetTranslation(vocab.Translation ?? string.Empty, vocab.Description);

            updatedCount++;
        }

        var newVocabs = normalizedVocabs
            .Where(vocab => !existingVocabMap.ContainsKey(vocab.Word.ToLower()))
            .Select(vocab => new Vocab(
                languageId,
                vocab.Word,
                vocab.Level!.Value
            ))
            .ToList();

        if (newVocabs.Count == 0 && updatedCount == 0)
        {
            return 0;
        }

        dbContext.Vocabs.AddRange(newVocabs);
        await dbContext.SaveChangesAsync(cancellationToken);

        return newVocabs.Count + updatedCount;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static Task<List<VocabImportItem>> ReadVocabsAsync(
        TextReader reader,
        CancellationToken cancellationToken)
    {
        return CsvImportReader.ReadAsync(
            reader,
            IsHeaderRow,
            columns => CsvImportReader.BuildHeaderIndexes(
                columns,
                new Dictionary<string, string>
                {
                    ["typeid"] = "type",
                    ["vocabtype"] = "type",
                    ["levelid"] = "level",
                    ["vocablevel"] = "level",
                    ["exampletranslation"] = "exampleTranslation",
                    ["voiceref"] = "voiceId"
                }),
            MapRow,
            cancellationToken);
    }

    private static VocabImportItem? MapRow(
        IReadOnlyList<string> columns,
        IReadOnlyDictionary<string, int>? headerIndexes,
        int rowNumber)
    {
        var word = CsvImportReader.GetColumn(columns, headerIndexes, "word", 0);
        if (string.IsNullOrWhiteSpace(word))
        {
            return null;
        }

        var translation = CsvImportReader.RequireColumn(columns, headerIndexes, "translation", 1, rowNumber);
        var level = CsvImportReader.ParseOptionalEnum<VocabLevel>(
            CsvImportReader.GetColumn(columns, headerIndexes, "level", 6),
            "level",
            rowNumber);

        return new VocabImportItem(word, translation, level);
    }

    private static bool IsHeaderRow(IReadOnlyList<string> columns)
    {
        var headers = columns
            .Select(CsvImportReader.NormalizeHeader)
            .ToList();

        return headers.FirstOrDefault() == "word" ||
               headers.Count(header => header is "translation" or "type" or "typeid" or "vocabtype") >= 2;
    }
}
