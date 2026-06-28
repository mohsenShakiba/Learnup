using Learnup.Domain.AggregateRoots.Courses;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Stories;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Domain.AggregateRoots.Lessons;

public class Lesson
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public int Order { get; private set; }
    public LessonStatus Status { get; private set; }
    
    public int? Duration { get; private set; }

    public List<LessonStory> Stories { get; private set; }
    public List<LessonGrammar> Grammars { get; private set; }
    public List<LessonVocab> Vocabs { get; private set; }
    public List<UserLesson> Users { get; private set; }
    public List<Test> Tests { get; private set; }

    public int CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
}