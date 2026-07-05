using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.LeitnerBoxes;

public sealed record RemoveVocabFromLeitnerBox(int VocabId) : IRequest;

internal sealed class RemoveVocabFromLeitnerBoxHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<RemoveVocabFromLeitnerBox>
{
    public async Task<Unit> Handle(RemoveVocabFromLeitnerBox request, CancellationToken cancellationToken)
    {
        var item = await dbContext.LeitnerBoxes
        .Where(l => l.UserId == identityProvider.UserId)
        .SelectMany(l => l.Items)
        .FirstOrDefaultAsync(l => l.VocabId == request.VocabId);

        if (item is null)
        {
            return Unit.Value;
        }

        dbContext.LeitnerBoxItems.Remove(item);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
