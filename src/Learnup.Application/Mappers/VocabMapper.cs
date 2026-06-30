using Learnup.Application.Responses.Public.Vocabs;
using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Mappers;

public static class VocabMapper
{
    public static VocabResponse ToResponse(
        this Vocab vocab) =>
        new(
            vocab.Id,
            vocab.Word,
            vocab.Translation,
            vocab.VoiceId,
            vocab.Description,
            vocab.Level,
            vocab.Senses.Select(ToResponse).ToList()
        );

    public static VocabSenseResponse ToResponse(this VocabSense type)
    {
        return new VocabSenseResponse(type.Id, type.Translation, type.Description, type.Example, type.ExampleTranslation, type.Type);
    }
}