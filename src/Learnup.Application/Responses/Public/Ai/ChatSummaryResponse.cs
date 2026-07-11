namespace Learnup.Application.Responses.Public.Ai;

public sealed record ChatSummaryResponse(
    int Id,
    string? Title,
    DateTime CreatedAt,
    DateTime UpdatedAt);
