namespace Learnup.Application.ExternalServices;

public interface IVocabTranslationProvider
{
    Task<TranslationResult> GetTranslationAsync(string content, CancellationToken cancellationToken = default);
}

public record TranslationResult(string Translation, string? Description, string? ParentWord);