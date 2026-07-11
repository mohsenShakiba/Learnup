namespace Learnup.Application.Requests.Admin.Conversations;

public record ConversationRequest(string Title, List<string> Words, List<ConversationItemRequest> Sentences);
