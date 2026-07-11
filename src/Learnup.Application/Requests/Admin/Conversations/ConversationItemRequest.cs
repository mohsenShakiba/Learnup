namespace Learnup.Application.Requests.Admin.Conversations;

public record ConversationItemRequest(int Order, string Text, int Person, string? Translation);
