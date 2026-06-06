using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Grammars;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Grammars;

public sealed record GetGrammarById(int Id) : IRequest<GrammarResponse?>;

internal sealed class GetGrammarByIdHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetGrammarById, GrammarResponse?>
{
    public async Task<GrammarResponse?> Handle(
        GetGrammarById request,
        CancellationToken cancellationToken)
    {
        var grammar = await dbContext.Grammars
            .AsNoTracking()
            .Include(currentGrammar => currentGrammar.PrerequisiteGrammars)
            .Include(currentGrammar => currentGrammar.Lessons)
            .FirstOrDefaultAsync(currentGrammar => currentGrammar.Id == request.Id, cancellationToken);

        return grammar?.ToResponse();
    }
}
