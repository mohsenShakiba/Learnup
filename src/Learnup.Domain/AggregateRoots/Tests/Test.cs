using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Domain.AggregateRoots.Tests;

public class Test
{
    public int Id { get; private set; }
    public int LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = null!;
    public TestType Type { get; private set; }
    public TestQuestionType QuestionType { get; private set; }
    public string Question { get; private set; } = null!;
    public TestStatus Status { get; private set; }
    public string? VoiceId { get; private set; }

    private readonly List<TestOption> _options = [];
    public IReadOnlyList<TestOption> Options => _options.AsReadOnly();

    private readonly List<UserTestResult> _userTestResults = [];
    public IReadOnlyList<UserTestResult> UserTestResults => _userTestResults.AsReadOnly();

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