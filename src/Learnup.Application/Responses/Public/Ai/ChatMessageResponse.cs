namespace Learnup.Application.Responses.Public.Ai;

public sealed record ChatMessageResponse(
    int Id,
    string Role,
    string Content,
    DateTime CreatedAt);
