namespace Learnup.Domain.AggregateRoots.Lessons;

public enum LessonStatus
{
    SoftDeleted = -1,
    Pending = 0,
    Published = 1,
}