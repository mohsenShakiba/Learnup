using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Application.Responses.Public.Lessons;

public sealed record LessonResponse(
    int Id,
    string Title,
    int Order,
    LessonStatus Status,
    int CourseId);

public sealed record LessonDetailResponse(
    int Id,
    string Title,
    int Order,
    LessonStatus Status,
    int CourseId,
    IReadOnlyList<int> StoryIds,
    IReadOnlyList<int> GrammarIds,
    IReadOnlyList<int> VocabIds);
