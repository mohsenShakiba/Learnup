using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.ExternalServices;

public interface IVocabLoader
{
    Task<int> LoadAsync(
        IReadOnlyCollection<VocabImportItem> vocabs,
        int defaultLevelId,
        int languageId,
        CancellationToken cancellationToken = default);
}

public sealed record VocabImportItem(
    string Word,
    string? Translation,
    VocabType Type,
    VocabLevel? Level = null,
    string? Description = null,
    string? Example = null,
    string? ExampleTranslation = null,
    string? VoiceId = null);
