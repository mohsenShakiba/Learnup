using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Application.Responses.Public.Lessons;

public record UserLessonResponse(
    UserLessonStatus Status,
    bool IsStoryCompleted,
    bool IsGrammarCompleted,
    bool IsVocabCompleted,
    bool IsGrammarTestCompleted,
    bool IsVocabTestCompleted);