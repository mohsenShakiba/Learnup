namespace Learnup.Application.Responses.Public.AudioBooks;

public sealed record AudioBookResponse(
    int Id,
    string Title,
    string? Description,
    string? Author,
    string? Level,
    string? Year,
    string? WordCount,
    string? Source,
    string? CoverId,
    bool IsVoiced);

public sealed record AudioBookDetailResponse(
    int Id,
    string Title,
    string? Description,
    string? Author,
    string? Level,
    string? Year,
    string? WordCount,
    string? Source,
    string? Content,
    string? CoverId,
    string? VoiceId,
    string? TimingJsonId,
    bool IsVoiced,
    IReadOnlyList<AudioBookItemResponse> Items);

public sealed record AudioBookItemResponse(
    int Id,
    string Sentence,
    string? Translation,
    int Order,
    IReadOnlyList<AudioBookItemExpressionResponse> Expressions);

public sealed record AudioBookItemExpressionResponse(
    int Id,
    string Phrase,
    string Meaning,
    string? Translation);
