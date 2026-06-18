using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Application.Responses.Public.LeitnerBox;

public sealed record BoxLevelResponse(
    int Id,
    IReadOnlyList<BoxLevelInfoResponse> Levels);

public sealed record BoxLevelInfoResponse(
    int Id,
    Level Level,
    TimeSpan WillReviewedIn,
    int ItemsCount,
    int DueItemsCount,
    DateTime? NextReviewAt);
