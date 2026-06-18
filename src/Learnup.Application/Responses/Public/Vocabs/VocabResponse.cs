using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Responses.Public.Vocabs;

public sealed record VocabResponse(
    int Id,
    string Word,
    string? Translation,
    string? VoiceId,
    string? Description,
    string? Example,
    string? ExampleTranslation,
    VocabLevel Level,
    VocabStatus Status,
    VocabType Type,
    int LanguageId);

public sealed record VocabTranslationResponse(
    int Id,
    string Translation,
    string? Description,
    VocabType Type,
    string Example,
    string ExampleTranslation);
