namespace Learnup.Application.Responses.Public.Files;

public sealed record FileResponse(
    string Id,
    string Path,
    string ContentType);
