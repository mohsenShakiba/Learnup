namespace Learnup.Application.Responses.Public.Conversations;

public sealed record ConversationResponse(
    int Id,
    string Title,
    string? Description,
    int? Duration,
    bool IsCompleted,
    IReadOnlyList<ConversationItemResponse> Items);

public sealed record ConversationItemResponse(
    int Id,
    string Content,
    string? Translation,
    int Order,
    int Person);

public sealed record ConversationItemExpressionResponse(
    int Id,
    string Phrase,
    string Meaning,
    string? Translation);


