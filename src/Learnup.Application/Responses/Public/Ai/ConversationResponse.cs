namespace Learnup.Application.Responses.Public.Ai;

public sealed record ConversationResponse(
    int Id,
    string? Title,
    DateTime CreatedAt,
    DateTime UpdatedAt);
