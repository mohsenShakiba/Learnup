using Learnup.Application.ExternalServices;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.ExternalService;

public class VocabLoader(LearnupDbContext dbContext) : IVocabLoader
{
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
                Example = NormalizeOptional(vocab.Example),
                ExampleTranslation = NormalizeOptional(vocab.ExampleTranslation),
                VoiceId = NormalizeOptional(vocab.VoiceId),
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
                vocab => (Word: vocab.Word.ToLower(), vocab.Level),
                vocab => vocab);

        var updatedCount = 0;

        foreach (var vocab in normalizedVocabs)
        {
            if (!existingVocabMap.TryGetValue((vocab.Word.ToLower(), vocab.Level!.Value), out var existingVocab))
            {
                continue;
            }

            existingVocab.UpdateImportDetails(
                vocab.Translation,
                vocab.Type,
                vocab.Description,
                vocab.Example,
                vocab.ExampleTranslation,
                vocab.VoiceId);

            updatedCount++;
        }

        var newVocabs = normalizedVocabs
            .Where(vocab => !existingVocabMap.ContainsKey((vocab.Word.ToLower(), vocab.Level!.Value)))
            .Select(vocab => new Vocab(
                languageId,
                vocab.Word,
                vocab.Translation,
                vocab.Type,
                vocab.Level!.Value,
                vocab.Description,
                vocab.Example,
                vocab.ExampleTranslation,
                vocab.VoiceId))
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
}
