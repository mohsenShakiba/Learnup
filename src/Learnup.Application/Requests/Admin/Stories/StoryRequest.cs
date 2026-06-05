namespace Learnup.Application.Requests.Admin.Stories;

public record StoryRequest(string Title, List<string> Words, List<StoryItemRequest> Sentences);
