using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserLesson
{
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = null!;

    public DateTime StartedAt { get; private set; }
    public DateTime LastVisitedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public UserLessonEntityType? LastReadEntityType { get; private set; }
    public int? LastReadEntityId { get; private set; }

    public UserLesson(int userId, int lessonId)
    {
        UserId = userId;
        LessonId = lessonId;
        StartedAt = LastVisitedAt = DateTime.UtcNow;
    }

    public void Touch()
    {
        LastVisitedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        CompletedAt ??= DateTime.UtcNow;
        Touch();
    }

    public void TrackLastReadEntity(UserLessonEntityType entityType, int entityId)
    {
        LastReadEntityType = entityType;
        LastReadEntityId = entityId;
        Touch();
    }
}
