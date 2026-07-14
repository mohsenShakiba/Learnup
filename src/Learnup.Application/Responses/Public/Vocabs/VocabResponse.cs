using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Responses.Public.Vocabs;

public sealed record VocabResponse(
    int Id,
    string Word,
    string? Translation,
    string? VoiceId,
    string? Description,
    string? ParentVocab,
    string? ParentVocabDescription,
    VocabLevel Level,
    bool IsInLeitnerBox,
    List<VocabSenseResponse> Senses);

public sealed record VocabSenseResponse(
    int Id,
    string? Translation,
    string? Description,
    string? Example,
    string? ExampleTranslation,
    VocabType Type
  );
