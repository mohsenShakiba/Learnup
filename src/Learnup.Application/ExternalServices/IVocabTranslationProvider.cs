namespace Learnup.Application.ExternalServices;

using Learnup.Domain.AggregateRoots.Vocabularies;

public interface IVocabTranslationProvider
{
    Task<TranslationResult> GetTranslationAsync(string content, CancellationToken cancellationToken = default);
    Task<VocabTransactionResult> GetVocabTranslationAsync(
        string word,
        VocabTranslationType type,
        CancellationToken cancellationToken = default);
}

public record TranslationResult(
    string Translation,
    string? Description,
    string? ParentWord, 
    string[] Types);


public record VocabTransactionResult(
    string Translation,
    string? Description,
    string Type,
    string Example,
    string ExampleTranslation);
