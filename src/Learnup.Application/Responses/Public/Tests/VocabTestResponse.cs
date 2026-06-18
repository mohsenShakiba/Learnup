using Learnup.Domain.AggregateRoots.Tests;

namespace Learnup.Application.Responses.Public.Tests;



public sealed record VocabTestResponse(
    int Id,
    int VocabId,
    string Question,
    VocabTestType Type,
    IReadOnlyList<TestOptionResponse> Options,
    int? UserSelectedOptionId,
    bool? IsCorrect);

public sealed record TestOptionResponse(int Id, string Text);
