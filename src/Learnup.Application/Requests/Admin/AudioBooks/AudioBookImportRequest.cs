namespace Learnup.Application.Requests.Admin.AudioBooks;

public sealed record AudioBookImportItemRequest(
    string Sentence,
    string? Translation);

public sealed record AudioBookImportRequest(
    string Title,
    string? Description,
    string? Author,
    string? Year,
    string? Level,
    string? WordCount,
    string? Source,
    string Content,
    IReadOnlyList<AudioBookImportItemRequest> Items);
