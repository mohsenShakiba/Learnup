using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Domain.AggregateRoots.Tests;

public class Test
{
    private readonly List<TestOption> _options = [];

    public int Id { get; private set; }
    public int LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = null!;
    public TestType Type { get; private set; }
    public TestQuestionType QuestionType { get; private set; }
    public string Question { get; private set; } = null!;
    public TestStatus Status { get; private set; }

    public IReadOnlyList<TestOption> Options => _options.AsReadOnly();

    private Test()
    {
    }

    public Test(int lessonId, TestType type)
    {
        LessonId = lessonId;
        Type = type;
        Status = TestStatus.Pending;
    }

    public void Publish(TestQuestionType questionType, string question, IEnumerable<TestOption> options)
    {
        QuestionType = questionType;
        Question = question;
        _options.Clear();
        _options.AddRange(options);
        Status = TestStatus.Published;
    }
}
