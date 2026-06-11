namespace Learnup.Application.ExternalServices;

public interface IVocabTranslationProvider
{
    Task<TranslationResult> GetTranslationAsync(string content, CancellationToken cancellationToken = default);
}

public record TranslationResult(
    string Translation,
    string? Description,
    string? ParentWord,
    IReadOnlyList<VocabTransactionResult> Transactions);

public record VocabTransactionResult(
    string Translation,
    string? Description,
    string Type,
    string Example,
    string ExampleTranslation);
