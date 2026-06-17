using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.LeitnerBox;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.LeitnerBox;

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
            .FirstOrDefaultAsync(b => b.UserId == identityProvider.UserId, cancellationToken);

        if (box is null)
            return null;

        return new LeitnerBoxResponse(
            box.Id,
            box.Items.Select(i => new LeitnerBoxItemResponse(
                i.Id,
                i.VocabId,
                i.Vocab.Word,
                i.Vocab.Translation,
                i.BoxLevel,
                i.AddedAt)).ToList());
    }
}
