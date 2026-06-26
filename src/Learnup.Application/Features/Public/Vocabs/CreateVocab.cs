using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Features.Public.Vocabs;

public sealed record CreateVocab(
    int LanguageId,
    string Word,
    string? Translation,
    VocabType Type,
    VocabLevel Level,
    string? Description,
    string? Example,
    string? ExampleTranslation,
    string? VoiceId) : IRequest<int>;

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

        var vocab = new Vocab(
            request.LanguageId,
            word,
            request.Translation,
            request.Type,
            request.Level,
            request.Description,
            request.Example,
            request.ExampleTranslation,
            request.VoiceId);

        dbContext.Vocabs.Add(vocab);
        await dbContext.SaveChangesAsync(cancellationToken);

        return vocab.Id;
    }
}
