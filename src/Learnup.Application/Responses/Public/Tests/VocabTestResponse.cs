namespace Learnup.Application.Responses.Public.Tests;

public sealed record VocabTestResponse(
    int Id,
    int VocabId,
    string Question,
    IReadOnlyList<TestOptionResponse> Options,
    int? UserSelectedOptionId,
    bool? IsCorrect);

public sealed record TestOptionResponse(int Id, string Text);
