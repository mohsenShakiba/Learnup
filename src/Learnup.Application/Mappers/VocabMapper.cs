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
            vocab.LanguageId);
}
