using Learnup.Domain.AggregateRoots.Users;

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
