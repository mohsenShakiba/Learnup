namespace Learnup.Domain.AggregateRoots.InvalidData;

/// <summary>
/// Identifies what <see cref="InvalidDataReport.TargetId"/> refers to
/// within the selected section (e.g. a lesson vocab under Courses).
/// </summary>
public enum InvalidDataReportTargetType
{
    Course = 1,
    Lesson = 2,
    Conversation = 3,
    ConversationItem = 4,
    ConversationItemExpression = 5,
    Grammar = 6,
    Vocab = 7,
    Test = 8,
    Ebook = 9,
    LeitnerBox = 10,
    LeitnerBoxItem = 11,
    AudioBook = 12,
    AudioBookListItem = 13,
    PlacementTest = 14,
    PlacementQuestion = 15,
    Chat = 16,
    Subscription = 17
}
