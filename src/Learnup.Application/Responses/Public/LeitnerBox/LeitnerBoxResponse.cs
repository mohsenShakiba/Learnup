using Learnup.Domain.AggregateRoots.Users;
using Learnup.Application.Responses.Public.Vocabs;

namespace Learnup.Application.Responses.Public.LeitnerBox;

public sealed record LeitnerBoxResponse(
    int Id,
    IReadOnlyList<LeitnerBoxItemResponse> Items);

public sealed record LeitnerBoxItemResponse(
    int Id,
    int VocabId,
    string Word,
    string? Translation,
    Level BoxLevel,
    DateTime AddedAt);

public sealed record DueLeitnerBoxItemResponse(
    int Id,
    int VocabId,
    string Word,
    string? Translation,
    string? Description,
    string? VoiceId,
    List<VocabSenseResponse> Senses,
    DateTime? NextReviewAt);
