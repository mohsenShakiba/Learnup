
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
    DateTime? LastVisitedAt);