namespace Learnup.Application.Responses.Public.Ai;

public sealed record ChatResponse(
    int ChatId,
    string Reply,
    int TokensUsed);
