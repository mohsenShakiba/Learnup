using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Grammars;

public record DeleteGrammar(int GrammarId) : IRequest;

public class DeleteGrammarHandler(ILearnupDbContext context) : IRequestHandler<DeleteGrammar>
{
    public async Task<Unit> Handle(DeleteGrammar request, CancellationToken cancellationToken)
    {
        var grammar = await context.Grammars
            .FirstOrDefaultAsync(g => g.Id == request.GrammarId, cancellationToken);

        if (grammar == null)
        {
            return Unit.Value;
        }
        
        context.Grammars.Remove(grammar);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}