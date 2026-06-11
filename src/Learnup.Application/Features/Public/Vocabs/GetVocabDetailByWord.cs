using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Vocabs;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Vocabs;

public sealed record GetVocabDetailByWord(string Word) : IRequest<VocabDetailResponse?>;

internal sealed class GetVocabDetailByWordHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetVocabDetailByWord, VocabDetailResponse?>
{
    public async Task<VocabDetailResponse?> Handle(
        GetVocabDetailByWord request,
        CancellationToken cancellationToken)
    {
        var word = request.Word.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(word))
            return null;

        var vocab = await dbContext.Vocabs
            .AsNoTracking()
            .Include(v => v.Language)
            .Where(v => v.Word.ToLower() == word)
            .FirstOrDefaultAsync(cancellationToken);

        if (vocab is null)
        {
            return null;
        }

        var translations = await dbContext.VocabTransactions
            .AsNoTracking()
            .Where(translation => translation.VocabId == vocab.Id)
            .ToListAsync(cancellationToken);

        return new VocabDetailResponse(
            vocab.Id,
            vocab.Word,
            vocab.Translation,
            vocab.VoiceId,
            vocab.Description,
            vocab.Level,
            vocab.LanguageId,
            vocab.Language.Name,
            translations.Select(translation => translation.ToResponse()).ToList());
    }
}
