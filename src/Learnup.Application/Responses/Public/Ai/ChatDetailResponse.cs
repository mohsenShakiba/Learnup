namespace Learnup.Application.Responses.Public.Ai;

public sealed record ChatDetailResponse(
    int Id,
    string? Title,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<ChatMessageResponse> Messages);
