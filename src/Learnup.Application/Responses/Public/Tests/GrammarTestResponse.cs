namespace Learnup.Application.Responses.Public.Tests;

public sealed record GrammarTestResponse(
    int Id,
    int GrammarId,
    string Question,
    IReadOnlyList<TestOptionResponse> Options,
    int? UserSelectedOptionId,
    bool? IsCorrect);
