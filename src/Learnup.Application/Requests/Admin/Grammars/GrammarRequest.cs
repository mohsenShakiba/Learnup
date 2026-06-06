using Learnup.Domain.AggregateRoots.Grammars;

namespace Learnup.Application.Requests.Admin.Grammars;

public record GrammarRequest(
    string Name,
    int LevelId,
    int Order,
    int EstimatedTimeMinutes,
    string Description,
    int? ParentGrammarId,
    List<GrammarLessonRequest> Lessons);

public record GrammarLessonRequest(
    string Title,
    HTMLTag HtmlTag,
    string Content,
    int Order,
    string Language,
    int? VoiceId);
