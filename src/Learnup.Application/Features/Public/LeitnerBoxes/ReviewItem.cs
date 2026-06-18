using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.LeitnerBoxes;

public record ReviewItem(int LeitnerBoxItemId, AnswerQuality AnswerQuality) : IRequest;

public class ReviewItemHandler(ILearnupDbContext context, IIdentityProvider identityProvider) : IRequestHandler<ReviewItem>
{
    public async Task<Unit> Handle(ReviewItem request, CancellationToken cancellationToken)
    {
        var box = await context.LeitnerBoxes
            .Include(lb => lb.Items)
            .ThenInclude(i => i.BoxLevel)
            .Include(lb => lb.BoxLevels)
            .FirstOrDefaultAsync(lb => lb.UserId == identityProvider.UserId, cancellationToken);

        if (box is null)
        {
            throw new InvalidOperationException("Leitner box not found");
        }

        var boxItem = box.Items.FirstOrDefault(i => i.Id == request.LeitnerBoxItemId);
        if (boxItem is null)
        {
            throw new InvalidOperationException("Leitner box item not found");
        }

        var currentLevelValue = (int)boxItem.BoxLevel.Level;
        var targetLevelValue = request.AnswerQuality switch
        {
            AnswerQuality.NoIdea => (int)Level.Level_1,
            AnswerQuality.Hard => Math.Max((int)Level.Level_1, currentLevelValue - 1),
            AnswerQuality.Mild => Math.Min((int)Level.Level_15, currentLevelValue + 1),
            AnswerQuality.PeaceOfCake => Math.Min((int)Level.Level_15, currentLevelValue + 2),
            _ => currentLevelValue
        };

        var targetLevel = box.BoxLevels.FirstOrDefault(level => (int)level.Level == targetLevelValue);
        if (targetLevel is null)
        {
            throw new InvalidOperationException("Target box level not found");
        }

        boxItem.ChangeBoxLevel(targetLevel);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
