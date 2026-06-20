using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.MotivationalSentences;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.MotivationalSentences;

public sealed record GetMotivationalSentence : IRequest<MotivationalSentenceResponse?>;

internal sealed class GetMotivationalSentenceHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetMotivationalSentence, MotivationalSentenceResponse?>
{
    public Task<MotivationalSentenceResponse?> Handle(
        GetMotivationalSentence request,
        CancellationToken cancellationToken)
    {
        return dbContext.MotivationalSentences
            .AsNoTracking()
            .Where(sentence => sentence.IsActive)
            .OrderBy(_ => EF.Functions.Random())
            .Select(sentence => new MotivationalSentenceResponse(
                sentence.Id,
                sentence.Sentence))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
