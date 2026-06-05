using Learnup.Domain.AggregateRoots.Languages;

namespace Learnup.Domain.AggregateRoots.Courses;

public class Course
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public int Order { get; private set; }
    
    public int LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;
}