namespace Learnup.Application.Responses.Public.Files;

public sealed record FileResponse(
    string Id,
    Stream Content,
    string ContentType);
