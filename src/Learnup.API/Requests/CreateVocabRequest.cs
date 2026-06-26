using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.API.Requests;

public sealed record CreateVocabRequest(
    int LanguageId,
    string Word,
    string? Translation,
    VocabType Type,
    VocabLevel Level,
    string? Description,
    string? Example,
    string? ExampleTranslation,
    string? VoiceId);
