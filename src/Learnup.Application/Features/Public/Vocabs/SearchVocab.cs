using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Vocabs;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Vocabs;

public sealed record SearchVocab(string Word) : IRequest<List<VocabResponse>>;

internal sealed class SearchVocabHandler(ILearnupDbContext dbContext)
    : IRequestHandler<SearchVocab, List<VocabResponse>>
{
    public async Task<List<VocabResponse>> Handle(SearchVocab request, CancellationToken cancellationToken)
    {
        var word = request.Word.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(word))
        {
            return [];
        }

        var vocabs = await dbContext.Vocabs
            .AsNoTracking()
            .Include(v => v.Senses)
            .Where(vocab => vocab.Word.ToLower() == word.ToLower())
            .ToListAsync(cancellationToken);

        return vocabs.Select(v => v.ToResponse()).ToList();
    }
}
