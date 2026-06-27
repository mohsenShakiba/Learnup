using Learnup.Domain.AggregateRoots.Tests;

namespace Learnup.Application.Responses.Public.Tests;

public sealed record TestResponse(
    int Id,
    int LessonId,
    TestType Type,
    TestQuestionType QuestionType,
    string Question,
    IReadOnlyList<TestOptionResponse> Options,
    int? UserSelectedOptionId,
    bool? IsCorrect);

public sealed record TestOptionResponse(int Id, string Text);
