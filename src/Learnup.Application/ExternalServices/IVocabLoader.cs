namespace Learnup.Application.ExternalServices;

public interface IVocabLoader
{
    Task<int> LoadAsync(string[] words, int levelId, int languageId, CancellationToken cancellationToken = default);
}
