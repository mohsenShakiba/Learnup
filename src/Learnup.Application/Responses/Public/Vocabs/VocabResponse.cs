using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Responses.Public.Vocabs;

public sealed record VocabResponse(
    int Id,
    string Word,
    string? Translation,
    string? VoiceId,
    string? Description,
    VocabLevel Level,
    int LanguageId,
    IReadOnlyList<VocabTranslationResponse> Translations);

public sealed record VocabTranslationResponse(
    int Id,
    string Translation,
    string? Description,
    VocabTranslationType Type,
    string Example,
    string ExampleTranslation);
