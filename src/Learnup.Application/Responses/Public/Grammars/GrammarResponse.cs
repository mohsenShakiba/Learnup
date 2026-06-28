using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.Responses.Public.Grammars;

public sealed record GrammarResponse(
    int Id,
    string Name,
    VocabLevel Level,
    int Order,
    int Duration,
    string Description,
    int? ParentGrammarId,
    IReadOnlyList<int> PrerequisiteGrammarIds,
    IReadOnlyList<GrammarLessonResponse> Lessons);

public sealed record GrammarLessonResponse(
    int Id,
    string Title,
    HTMLTag HtmlTag,
    string Content,
    int Order,
    string Language,
    int? VoiceId);
