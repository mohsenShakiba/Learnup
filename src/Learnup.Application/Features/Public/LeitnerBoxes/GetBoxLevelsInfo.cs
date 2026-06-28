using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.LeitnerBox;
using Learnup.Domain.AggregateRoots.LeitnerBoxes;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.LeitnerBoxes;

public sealed record GetBoxLevelsInfo : IRequest<BoxLevelResponse>;

internal sealed class GetBoxLevelsInfoHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetBoxLevelsInfo, BoxLevelResponse>
{
    public async Task<BoxLevelResponse> Handle(GetBoxLevelsInfo request, CancellationToken cancellationToken)
    {
        var box = await dbContext.LeitnerBoxes
            .AsNoTracking()
            .Include(b => b.BoxLevels)
            .Include(b => b.Items)
            .ThenInclude(i => i.BoxLevel)
            .FirstOrDefaultAsync(b => b.UserId == identityProvider.UserId, cancellationToken);

        if (box is null)
        {
            box = LeitnerBox.CreateWithLevels(identityProvider.UserId);
            dbContext.LeitnerBoxes.Add(box);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var now = DateTime.UtcNow;

        return new BoxLevelResponse(
            box.Id,
            box.BoxLevels
                .OrderBy(level => (int)level.Level)
                .Select(level =>
                {
                    var levelItems = box.Items.Where(item => item.BoxLevelId == level.Id).ToList();

                    return new BoxLevelInfoResponse(
                        level.Id,
                        level.Level,
                        level.WillReviewedIn,
                        levelItems.Count,
                        levelItems.Count(item => item.NextReviewAt is not null && item.NextReviewAt <= now),
                        levelItems
                            .Where(item => item.NextReviewAt is not null)
                            .Select(item => item.NextReviewAt)
                            .Min());
                })
                .ToList());
    }
}