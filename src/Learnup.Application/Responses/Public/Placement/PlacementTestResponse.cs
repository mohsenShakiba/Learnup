using Learnup.Domain.AggregateRoots.Placement;

namespace Learnup.Application.Responses.Public.Placement;

public sealed record PlacementTestResponse(
    int Id,
    string Title,
    string Description,
    string Instructions,
    IReadOnlyList<PlacementQuestionResponse> Questions);

public sealed record PlacementQuestionResponse(
    int Id,
    int Number,
    string Level,
    PlacementSkill Skill,
    string Prompt,
    IReadOnlyList<PlacementOptionResponse> Options);

public sealed record PlacementOptionResponse(int Id, string Text);
