namespace Learnup.Application.Responses.Public.Users;

public sealed record UserBookResponse(
    int Id,
    string Title,
    string FileName,
    int CurrentPage,
    DateTime UploadedAt);
