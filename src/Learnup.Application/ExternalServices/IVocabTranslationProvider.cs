namespace Learnup.Application.ExternalServices;

using Learnup.Domain.AggregateRoots.Vocabularies;

public interface IVocabTranslationProvider
{
    Task<VocabTransactionResult> GetVocabTranslationAsync(
        string word,
        VocabType type,
        CancellationToken cancellationToken = default);
}

public record VocabTransactionResult(
    string Translation,
    string Example,
    string ExampleTranslation);
