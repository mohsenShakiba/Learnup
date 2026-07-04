using Learnup.Application.Responses.Public.Grammars;
using Learnup.Application.Responses.Public.Stories;
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
    List<StoryResponse> Stories,
    List<GrammarResponse> Grammars,
    List<VocabResponse> Vocabs,
    List<TestResponse> Tests);

public sealed record CurrentLessonProgressResponse(
    int LessonId,
    string Title,
    int Order,
    int CourseId,
    bool IsStoryCompleted,
    bool IsGrammarCompleted,
    bool IsVocabCompleted,
    bool IsTestCompleted,
    int? NextLessonId);
