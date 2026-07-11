using Learnup.Application.Responses.Public.Grammars;
using Learnup.Application.Responses.Public.Conversations;
using Learnup.Application.Responses.Public.Tests;
using Learnup.Application.Responses.Public.Vocabs;

namespace Learnup.Application.Responses.Public.Lessons;

public sealed record LessonResponse(
    int Id,
    string Title,
    int Order,
    int CourseId,
    bool IsCompleted)
{
}

public sealed record LessonDetailResponse(
    int Id,
    string Title,
    int Order,
    int CourseId,
    int? NextLessonId,
    UserLessonResponse UserLesson,
    List<ConversationResponse> Conversations,
    List<GrammarResponse> Grammars,
    List<VocabResponse> Vocabs,
    List<TestResponse> Tests);

public sealed record CurrentLessonProgressResponse(
    int LessonId,
    string Title,
string CourseCode,
string CourseSlug,
    int Order,
    int CourseId,
    bool IsConversationCompleted,
    bool IsGrammarCompleted,
    bool IsVocabCompleted,
    bool IsTestCompleted,
    int? NextLessonId);
