using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Vocabs;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Vocabs;

public sealed record GetVocabByWord(string Word) : IRequest<VocabResponse?>;

internal sealed class GetVocabByWordHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetVocabByWord, VocabResponse?>
{
    public async Task<VocabResponse?> Handle(
        GetVocabByWord request,
        CancellationToken cancellationToken)
    {
        var word = request.Word.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(word))
        {
            return null;
        }

        var vocab = await dbContext.Vocabs
            .AsNoTracking()
            .Where(vocab => vocab.Word.ToLower() == word)
            .FirstOrDefaultAsync(cancellationToken);

        if (vocab is null)
        {
            return null;
        }

        var translations = await dbContext.VocabTransactions
            .AsNoTracking()
            .Where(translation => translation.VocabId == vocab.Id)
            .ToListAsync(cancellationToken);

        return vocab.ToResponse(translations.Select(translation => translation.ToResponse()).ToList());
    }
}
