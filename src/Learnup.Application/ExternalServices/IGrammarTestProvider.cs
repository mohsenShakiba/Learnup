using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Stories;

namespace Learnup.Application.ExternalServices;

public interface IGrammarTestProvider
{
    Task<List<TestGenerationResult>> GetGrammarTestAsync(Grammar grammar, Story story, CancellationToken cancellationToken = default);
}
