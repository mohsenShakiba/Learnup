using Learnup.Application.Responses.Public.Grammars;
using Learnup.Application.Responses.Public.Stories;
using Learnup.Application.Responses.Public.Vocabs;

namespace Learnup.Application.Responses.Public.Lessons;

public sealed record LessonResponse(
    int Id,
    string Title,
    int Order,
    int CourseId);

public sealed record LessonDetailResponse(
    int Id,
    string Title,
    int Order,
    int CourseId,
    IReadOnlyList<StoryResponse> Stories,
    IReadOnlyList<GrammarResponse> Grammars,
    IReadOnlyList<VocabResponse> Vocabs);
