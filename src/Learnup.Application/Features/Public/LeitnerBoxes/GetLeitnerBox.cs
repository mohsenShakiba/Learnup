using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.LeitnerBox;
using Learnup.Domain.AggregateRoots.LeitnerBoxes;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.LeitnerBoxes;

public sealed record GetLeitnerBox : IRequest<LeitnerBoxResponse?>;

internal sealed class GetLeitnerBoxHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetLeitnerBox, LeitnerBoxResponse?>
{
    public async Task<LeitnerBoxResponse?> Handle(GetLeitnerBox request, CancellationToken cancellationToken)
    {
        var box = await dbContext.LeitnerBoxes
            .AsNoTracking()
            .Include(b => b.Items)
            .ThenInclude(i => i.Vocab)
            .Include(b => b.Items)
            .ThenInclude(i => i.BoxLevel)
            .FirstOrDefaultAsync(b => b.UserId == identityProvider.UserId, cancellationToken);

        if (box is null)
        {
            
            box = LeitnerBox.CreateWithLevels(identityProvider.UserId);
            dbContext.LeitnerBoxes.Add(box);
        }

        return new LeitnerBoxResponse(
            box.Id,
            box.Items.Select(i => new LeitnerBoxItemResponse(
                i.Id,
                i.VocabId,
                i.Vocab.Word,
                i.Vocab.Translation,
                i.BoxLevel.Level,
                i.AddedAt)).ToList());
    }
}
