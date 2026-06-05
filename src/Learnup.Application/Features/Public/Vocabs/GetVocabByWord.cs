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

        return await dbContext.Vocabs
            .AsNoTracking()
            .Where(vocab => vocab.Word.ToLower() == word)
            .Select(vocab => new VocabResponse(
                vocab.Id,
                vocab.Word,
                vocab.Translation,
                vocab.VoiceId,
                vocab.Description,
                vocab.Level,
                vocab.ParentVocabId,
                vocab.LanguageId))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
