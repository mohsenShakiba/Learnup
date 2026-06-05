namespace Learnup.Application.Responses.Public.Stories;

public sealed record StoryResponse(
    int Id,
    string Title,
    int? CoverId,
    IReadOnlyList<StoryItemResponse> Items);

public sealed record StoryItemResponse(
    int Id,
    string Content,
    string Translation,
    int Order,
    string? VoiceId);
