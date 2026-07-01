namespace Learnup.Application.Requests.Admin.Placement;

public record PlacementTestRequest(
    string Title,
    string Description,
    string Instructions,
    List<PlacementQuestionRequest> Questions);

public record PlacementQuestionRequest(
    int Id,
    string Level,
    string Skill,
    string Prompt,
    List<string> Options,
    string Answer);
