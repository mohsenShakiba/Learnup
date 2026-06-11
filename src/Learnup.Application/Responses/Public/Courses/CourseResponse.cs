using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Application.Responses.Public.Courses;

public sealed record CourseResponse(
    int Id,
    string Code,
    string Slug,
    string Title,
    string Description,
    int Order,
    int TotalLessonsCount,
    int CompletedLessonsCount,
    int LanguageId, 
    string? CoverId,
    DateTime? LastVisitedAt)

{
    
}


public sealed record CourseLessonResponse(
    int Id,
    string Title,
    int Order,
    LessonStatus Status,
    int StoriesCount,
    int GrammarsCount,
    int VocabsCount);
