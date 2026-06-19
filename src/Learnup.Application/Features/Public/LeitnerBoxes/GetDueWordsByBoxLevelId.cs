using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.LeitnerBox;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.LeitnerBoxes;

public sealed record GetDueWordsByBoxLevelId(int BoxLevelId) : IRequest<IReadOnlyList<DueLeitnerBoxItemResponse>?>;

internal sealed class GetDueWordsByBoxLevelIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetDueWordsByBoxLevelId, IReadOnlyList<DueLeitnerBoxItemResponse>?>
{
    public async Task<IReadOnlyList<DueLeitnerBoxItemResponse>?> Handle(GetDueWordsByBoxLevelId request, CancellationToken cancellationToken)
    {
        var box = await dbContext.LeitnerBoxes
            .AsNoTracking()
            .Include(b => b.BoxLevels)
            .Include(b => b.Items)
            .ThenInclude(i => i.Vocab)
            .FirstOrDefaultAsync(b => b.UserId == identityProvider.UserId, cancellationToken);

        if (box is null || box.BoxLevels.All(level => level.Id != request.BoxLevelId))
        {
            return null;
        }

        var now = DateTime.UtcNow;

        return box.Items
            .Where(item => item.BoxLevelId == request.BoxLevelId && item.NextReviewAt is not null && item.NextReviewAt <= now)
            .OrderBy(item => item.NextReviewAt)
            .Select(item => new DueLeitnerBoxItemResponse(
                item.Id,
                item.VocabId,
                item.Vocab.Word,
                item.Vocab.Translation,
                item.NextReviewAt))
            .ToList();
    }
}
