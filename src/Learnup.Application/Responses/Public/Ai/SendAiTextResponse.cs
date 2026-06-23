namespace Learnup.Application.Responses.Public.Ai;

public sealed record SendAiTextResponse(
    string WordTranslation,
    string SentenceTranslation);
