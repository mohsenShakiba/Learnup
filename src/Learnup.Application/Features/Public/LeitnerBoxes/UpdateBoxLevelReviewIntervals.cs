using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Requests.Admin.Leitner;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.LeitnerBoxes;

public sealed record UpdateBoxLevelReviewIntervals(int BoxId, List<UpdateBoxLevelReviewIntervalRequest> Levels) : IRequest;

internal sealed class UpdateBoxLevelReviewIntervalsHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider) : IRequestHandler<UpdateBoxLevelReviewIntervals>
{
    public async Task<Unit> Handle(UpdateBoxLevelReviewIntervals request, CancellationToken cancellationToken)
    {
        var box = await dbContext.LeitnerBoxes
            .Include(b => b.BoxLevels)
            .Include(b => b.Items)
            .FirstOrDefaultAsync(
                b => b.Id == request.BoxId && b.UserId == identityProvider.UserId,
                cancellationToken);

        if (box is null)
        {
            throw new InvalidOperationException("Leitner box not found");
        }

        foreach (var levelRequest in request.Levels)
        {
            if (levelRequest.Number < 0)
            {
                throw new InvalidOperationException("Review interval cannot be negative.");
            }

            var boxLevel = box.BoxLevels.FirstOrDefault(level => level.Id == levelRequest.LevelId);
            if (boxLevel is null)
            {
                throw new InvalidOperationException($"Box level {levelRequest.LevelId} not found");
            }

            var reviewInterval = TimeSpan.FromDays(levelRequest.Number);
            boxLevel.UpdateWillReviewedIn(reviewInterval);

            foreach (var item in box.Items.Where(item => item.BoxLevelId == boxLevel.Id))
            {
                item.UpdateNextReviewAt(reviewInterval);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
