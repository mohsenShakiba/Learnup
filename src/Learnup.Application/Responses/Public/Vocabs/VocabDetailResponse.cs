using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Responses.Public.Vocabs;

public sealed record VocabDetailResponse(
    int Id,
    string Word,
    string? Translation,
    string? VoiceId,
    string? Description,
    VocalLevel Level,
    VocabResponse? ParentVocab,
    int LanguageId,
    string LanguageName);
