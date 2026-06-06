using Learnup.Application.Requests.Admin.Grammars;

namespace Learnup.Application.ExternalServices;

public interface IGrammarLoader
{
    Task<int> LoadAsync(GrammarRequest grammarRequest, CancellationToken cancellationToken = default);
}
