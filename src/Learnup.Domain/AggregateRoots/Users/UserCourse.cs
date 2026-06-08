using Learnup.Domain.AggregateRoots.Courses;

namespace Learnup.Domain.AggregateRoots.Users;

public class UserCourse
{
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int CourseId { get; private set; }
    public Course Course { get; private set; } = null!;

    public DateTime FirstVisitedAt { get; private set; }
    public DateTime LastVisitedAt { get; private set; }
    public int VisitCount { get; private set; }
}
