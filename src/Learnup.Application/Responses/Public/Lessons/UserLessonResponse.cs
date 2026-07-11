using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Application.Responses.Public.Lessons;

public record UserLessonResponse(
    UserLessonStatus Status,
    bool IsConversationCompleted,
    bool IsGrammarCompleted,
    bool IsVocabCompleted,
    bool IsTestCompleted);