using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.LeitnerBox;

public sealed record AddVocabToLeitnerBox(int VocabId) : IRequest;

internal sealed class AddVocabToLeitnerBoxHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<AddVocabToLeitnerBox>
{
    public async Task<Unit> Handle(AddVocabToLeitnerBox request, CancellationToken cancellationToken)
    {
        var box = await dbContext.LeitnerBoxes
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == identityProvider.UserId, cancellationToken);

        if (box is null)
        {
            box = new Domain.AggregateRoots.Users.LeitnerBox(identityProvider.UserId);
            dbContext.LeitnerBoxes.Add(box);
        }

        box.AddItem(request.VocabId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
