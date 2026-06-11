using Learnup.Application.Responses.Public.Vocabs;
using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Mappers;

public static class VocabMapper
{
    public static VocabResponse ToResponse(
        this Vocab vocab,
        IReadOnlyList<VocabTranslationResponse>? translations = null) =>
        new(
            vocab.Id,
            vocab.Word,
            vocab.Translation,
            vocab.VoiceId,
            vocab.Description,
            vocab.Level,
            vocab.LanguageId,
            translations ?? []);

    public static VocabTranslationResponse ToResponse(this VocabTransaction translation) =>
        new(
            translation.Id,
            translation.Translation,
            translation.Description,
            translation.Type,
            translation.Example,
            translation.ExampleTranslation);
}
