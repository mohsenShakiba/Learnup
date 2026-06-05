using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Responses.Public.Vocabs;

public sealed record VocabResponse(
    int Id,
    string Word,
    string? Translation,
    string? VoiceId,
    string? Description,
    VocalLevel Level,
    int? ParentVocabId,
    int LanguageId);
