namespace Learnup.Application.Responses.Public.Lessons;

public record UserLessonResponse(
    bool IsStoryCompleted,
    bool IsGrammarCompleted,
    bool IsVocabCompleted,
    bool IsGrammarTestCompleted,
    bool IsVocabTestCompleted);