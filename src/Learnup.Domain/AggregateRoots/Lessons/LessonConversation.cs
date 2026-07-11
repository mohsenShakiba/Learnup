using Learnup.Domain.AggregateRoots.Conversations;

namespace Learnup.Domain.AggregateRoots.Lessons;

public class LessonConversation
{
    public int LessonId { get; private set; }
    public int ConversationId { get; private set; }
    
    public Lesson Lesson { get; private set; }
    public Conversation Conversation { get; private set; }

    private LessonConversation()
    {
        Lesson = null!;
        Conversation = null!;
    }

    public LessonConversation(int lessonId, int conversationId)
    {
        LessonId = lessonId;
        ConversationId = conversationId;
        Lesson = null!;
        Conversation = null!;
    }
}
