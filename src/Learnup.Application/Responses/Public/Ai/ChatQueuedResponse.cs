namespace Learnup.Application.Responses.Public.Ai;

public sealed record ChatQueuedResponse(
    int? ChatId,
    string Status);
