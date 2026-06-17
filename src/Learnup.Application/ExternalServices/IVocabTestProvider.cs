namespace Learnup.Application.ExternalServices;

public interface IVocabTestProvider
{
    Task<TestGenerationResult> GetVocabTestAsync(string word, string translation, CancellationToken cancellationToken = default);
}
