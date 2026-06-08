using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Application.Responses.Public.Courses;

public sealed record CourseResponse(
    int Id,
    string Title,
    string Description,
    int Order,
    string? CoverId,
    int TotalLessonsCount,
    int CompletedLessonsCount,
    int LanguageId);


public sealed record CourseLessonResponse(
    int Id,
    string Title,
    int Order,
    LessonStatus Status,
    int StoriesCount,
    int GrammarsCount,
    int VocabsCount);
