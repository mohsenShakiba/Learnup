using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Grammars;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Grammars;

public record ListGrammarLessons(int GrammarId) : IRequest<List<GrammarLessonResponse>>;

public class ListGrammarLessonsHandler(ILearnupDbContext context) : IRequestHandler<ListGrammarLessons, List<GrammarLessonResponse>>
{
    public async Task<List<GrammarLessonResponse>> Handle(ListGrammarLessons request, CancellationToken cancellationToken)
    {
        var lessons = await context.GrammarLessons
            .Where(l => l.GrammarId == request.GrammarId)
            .Select(l => l.ToResponse())
            .ToListAsync(cancellationToken);

        return lessons;
    }
}