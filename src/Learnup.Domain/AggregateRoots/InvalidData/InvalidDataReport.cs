namespace Learnup.Domain.AggregateRoots.InvalidData;

using Learnup.Domain.AggregateRoots.Users;

/// <summary>
/// A user-submitted report about incorrect data or broken functionality
/// in a specific product section, optionally pointing at nested entities.
/// </summary>
public class InvalidDataReport
{
    public int Id { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public InvalidDataReportKind Kind { get; private set; }
    public InvalidDataReportSection Section { get; private set; }

    /// <summary>
    /// Optional course context when the report is under Courses.
    /// </summary>
    public int? CourseId { get; private set; }

    /// <summary>
    /// Optional lesson context nested under a course.
    /// </summary>
    public int? LessonId { get; private set; }

    /// <summary>
    /// The specific entity the report targets (vocab, ebook, etc.).
    /// </summary>
    public int? TargetId { get; private set; }

    public InvalidDataReportTargetType? TargetType { get; private set; }

    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private InvalidDataReport()
    {
        Text = null!;
    }

    public InvalidDataReport(
        int userId,
        InvalidDataReportKind kind,
        InvalidDataReportSection section,
        string text,
        int? courseId = null,
        int? lessonId = null,
        int? targetId = null,
        InvalidDataReportTargetType? targetType = null)
    {
        UserId = userId;
        Kind = kind;
        Section = section;
        Text = text;
        CourseId = courseId;
        LessonId = lessonId;
        TargetId = targetId;
        TargetType = targetType;
        CreatedAt = DateTime.UtcNow;
    }
}
