namespace Learnup.Domain.AggregateRoots.Lessons;

public enum LessonStatus
{
    SoftDeleted = -1,
    Pending = 0,
    Completed = 1,
    Published = 2,
}