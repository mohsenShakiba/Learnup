using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Grammars;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Grammars;

public record ListLessons() : IRequest<List<GrammarLessonResponse>>;

public class ListLessonsHandler(ILearnupDbContext context) : IRequestHandler<ListLessons, List<GrammarLessonResponse>>
{
    public async Task<List<GrammarLessonResponse>> Handle(ListLessons request, CancellationToken cancellationToken)
    {
        var lessons = await context.GrammarLessons
            .Select(l => l.ToResponse())
            .ToListAsync(cancellationToken);

        return lessons;
    }
}