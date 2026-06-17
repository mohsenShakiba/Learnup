namespace Learnup.Application.ExternalServices;

public interface IGrammarTestProvider
{
    Task<TestGenerationResult> GetGrammarTestAsync(string grammarName, string description, CancellationToken cancellationToken = default);
}
