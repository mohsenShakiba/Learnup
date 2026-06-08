using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Grammars;
using Learnup.Domain.AggregateRoots.Grammars;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Grammars;

public record ListGrammars : IRequest<List<GrammarResponse>>;

public class ListGrammarsHandler(ILearnupDbContext context) : IRequestHandler<ListGrammars, List<GrammarResponse>>
{
    public async Task<List<GrammarResponse>> Handle(ListGrammars request, CancellationToken cancellationToken)
    {
        var grammars = await context.Grammars
            .Include(g => g.Lessons)
            .Select(g => g.ToResponse()).ToListAsync(cancellationToken);
        
        return grammars;
    }
}