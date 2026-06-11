namespace Learnup.Application.Responses.Public.Stories;

public sealed record StoryResponse(
    int Id,
    string Title,
    string? CoverId,
    bool IsCompleted,
    IReadOnlyList<StoryItemResponse> Items);

public sealed record StoryItemResponse(
    int Id,
    string Content,
    string Translation,
    int Order,
    string? VoiceId,
    IReadOnlyList<StoryItemTimestampResponse> Timestamps);

public sealed record StoryItemTimestampResponse(
    int Id,
    string Word,
    float Start,
    float End);
