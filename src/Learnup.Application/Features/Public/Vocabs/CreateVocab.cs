using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Features.Public.Vocabs;

public sealed record CreateVocab(int LanguageId, string Word, string? Translation) : IRequest<int>;

internal sealed class CreateVocabHandler(ILearnupDbContext dbContext)
    : IRequestHandler<CreateVocab, int>
{
    public async Task<int> Handle(CreateVocab request, CancellationToken cancellationToken)
    {
        var word = request.Word.Trim();

        if (string.IsNullOrWhiteSpace(word))
        {
            throw new ArgumentException("Word is required.", nameof(request.Word));
        }

        var vocab = new Vocab(request.LanguageId, word, VocabLevel.Unknown, VocabSource.User);

        vocab.SetTranslation(request.Translation ?? "", null);

        dbContext.Vocabs.Add(vocab);

        await dbContext.SaveChangesAsync(cancellationToken);

        return vocab.Id;
    }
}