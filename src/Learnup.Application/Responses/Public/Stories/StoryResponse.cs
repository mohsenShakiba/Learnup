namespace Learnup.Application.Responses.Public.Stories;

public sealed record StoryResponse(
    int Id,
    string Title,
    string? Description,
    string? CoverId,
    int? Duration,
    bool IsCompleted,
    IReadOnlyList<StoryItemResponse> Items);

public sealed record StoryItemResponse(
    int Id,
    string Content,
    string? Translation,
    int Order,
    int Person,
    string? VoiceId);

public sealed record StoryItemExpressionResponse(
    int Id,
    string Phrase,
    string Meaning,
    string? Translation);


