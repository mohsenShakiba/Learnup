using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Responses.Public.Vocabs;

public sealed record VocabDetailResponse(
    int Id,
    string Word,
    string? Translation,
    string? VoiceId,
    string? Description,
    VocabLevel Level,
    int LanguageId,
    string LanguageName,
    IReadOnlyList<VocabTranslationResponse> Translations);
