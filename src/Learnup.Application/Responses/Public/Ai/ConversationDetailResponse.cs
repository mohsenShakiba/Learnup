namespace Learnup.Application.Responses.Public.Ai;

public sealed record ConversationDetailResponse(
    int Id,
    string? Title,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<ChatMessageResponse> Messages);
