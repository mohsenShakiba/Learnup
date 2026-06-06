using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Requests.Admin.Grammars;

namespace Learnup.Application.Features.Admin.Grammars;

public sealed record ImportGrammar(GrammarRequest Grammar) : IRequest<int>;

internal sealed class ImportGrammarHandler(IGrammarLoader grammarLoader) : IRequestHandler<ImportGrammar, int>
{
    public Task<int> Handle(ImportGrammar request, CancellationToken cancellationToken)
    {
        return grammarLoader.LoadAsync(request.Grammar, cancellationToken);
    }
}
