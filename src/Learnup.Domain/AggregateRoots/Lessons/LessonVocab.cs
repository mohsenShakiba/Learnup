using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Domain.AggregateRoots.Lessons;

public class LessonVocab
{
    public int LessonId { get; private set; }
    public int VocabId { get; private set; }
    
    public Lesson Lesson { get; private set; }
    public Vocab Vocab { get; private set; }

    private LessonVocab()
    {
        Lesson = null!;
        Vocab = null!;
    }

    public LessonVocab(int lessonId, int vocabId)
    {
        LessonId = lessonId;
        VocabId = vocabId;
        Lesson = null!;
        Vocab = null!;
    }
}
