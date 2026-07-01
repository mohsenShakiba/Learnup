using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Placement;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Placement;

public sealed record GetPlacementTest : IRequest<PlacementTestResponse?>;

internal sealed class GetPlacementTestHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetPlacementTest, PlacementTestResponse?>
{
    public async Task<PlacementTestResponse?> Handle(GetPlacementTest request, CancellationToken cancellationToken)
    {
        var placementTest = await dbContext.PlacementTests
            .AsNoTracking()
            .Include(test => test.Questions)
            .ThenInclude(question => question.Options)
            .FirstOrDefaultAsync(cancellationToken);

        return placementTest?.ToResponse();
    }
}
