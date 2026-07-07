namespace Learnup.API.Requests;

public sealed record ChatRequest(int? ConversationId, string Message);
