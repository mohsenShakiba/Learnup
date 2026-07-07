namespace Learnup.Application.Responses.Public.Ai;

public sealed record ChatResponse(
    int ConversationId,
    string Reply,
    int TokensUsed);
