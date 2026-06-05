using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Application.Responses.Public.Courses;

public sealed record CourseResponse(
    int Id,
    string Title,
    int Order,
    int LanguageId);

public sealed record CourseDetailResponse(
    int Id,
    string Title,
    int Order,
    int LanguageId,
    IReadOnlyList<CourseLessonResponse> Lessons);

public sealed record CourseLessonResponse(
    int Id,
    string Title,
    int Order,
    LessonStatus Status);
