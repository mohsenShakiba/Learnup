using Learnup.Application.ExternalServices;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.ExternalService;

public class VocabLoader(LearnupDbContext dbContext) : IVocabLoader
{
    public async Task<int> LoadAsync(
        string[] words,
        int levelId,
        int languageId,
        CancellationToken cancellationToken = default)
    {
        if (words.Length == 0)
        {
            return 0;
        }

        if (!Enum.IsDefined(typeof(VocabLevel), levelId))
        {
            throw new ArgumentOutOfRangeException(nameof(levelId), levelId, "Unknown vocab level id.");
        }

        var languageExists = await dbContext.Languages
            .AnyAsync(language => language.Id == languageId, cancellationToken);

        if (!languageExists)
        {
            throw new InvalidOperationException($"Language with id '{languageId}' was not found.");
        }

        var level = (VocabLevel)levelId;
        var normalizedWords = words
            .Select(word => word.Trim())
            .Where(word => !string.IsNullOrWhiteSpace(word))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedWords.Count == 0)
        {
            return 0;
        }

        var existingWords = await dbContext.Vocabs
            .Where(vocab => vocab.LanguageId == languageId && vocab.Level == level)
            .Select(vocab => vocab.Word)
            .ToListAsync(cancellationToken);

        var existingWordSet = existingWords.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var newVocabs = normalizedWords
            .Where(word => !existingWordSet.Contains(word))
            .Select(word => new Vocab(languageId, word, level))
            .ToList();

        if (newVocabs.Count == 0)
        {
            return 0;
        }

        dbContext.Vocabs.AddRange(newVocabs);
        await dbContext.SaveChangesAsync(cancellationToken);

        return newVocabs.Count;
    }
}
