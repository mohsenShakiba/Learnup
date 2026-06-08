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

        return await dbContext.Vocabs
            .AsNoTracking()
            .Include(v => v.Language)
            .Include(v => v.ParentVocab)
            .Where(v => v.Word.ToLower() == word)
            .Select(v => new VocabDetailResponse(
                v.Id,
                v.Word,
                v.Translation,
                v.VoiceId,
                v.Description,
                v.Level,
                v.ParentVocab == null ? null : new VocabResponse(
                    v.ParentVocab.Id,
                    v.ParentVocab.Word,
                    v.ParentVocab.Translation,
                    v.ParentVocab.VoiceId,
                    v.ParentVocab.Description,
                    v.ParentVocab.Level,
                    v.ParentVocab.ParentVocabId,
                    v.ParentVocab.LanguageId),
                v.LanguageId,
                v.Language.Name))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
