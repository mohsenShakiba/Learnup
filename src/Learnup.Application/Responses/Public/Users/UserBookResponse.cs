namespace Learnup.Application.Responses.Public.Users;

public sealed record UserBookResponse(
    int Id,
    string Title,
    string? Author,
    string FileName,
    string? CoverId,
    string? CurrentRef,
    float? Progress,
    DateTime UploadedAt);
