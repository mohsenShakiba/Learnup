
namespace Learnup.Application.Responses.Public.Courses;

public sealed record CourseResponse(
    int Id,
    string Code,
    string Slug,
    string Title,
    string Description,
string Brief,
    int Order,
    int TotalLessonsCount,
    int TotalStories,
    int TotalGrammars,
    int TotalVocabs,
    int CompletedLessonsCount,
    int LanguageId,
    DateTime? LastVisitedAt);
