using Learnup.Domain.AggregateRoots.Languages;
using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Courses;

public class Course
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int Order { get; private set; }
    public string? CoverId { get; private set; }

    public int LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;

    public List<Lesson> Lessons { get; private set; }
}