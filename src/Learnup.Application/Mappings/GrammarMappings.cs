using Learnup.Application.Responses.Public.Grammars;
using Learnup.Domain.AggregateRoots.Grammars;

namespace Learnup.Application.Mappings;

public static class GrammarMappings
{
    public static GrammarResponse ToResponse(this Grammar grammar)
    {
        return new GrammarResponse(
            grammar.Id,
            grammar.Name,
            grammar.Level,
            grammar.Order,
            grammar.Duration,
            grammar.Description,
            grammar.ParentGrammarId,
            grammar.PrerequisiteGrammars
                .Select(prerequisiteGrammar => prerequisiteGrammar.Id)
                .OrderBy(id => id)
                .ToArray(),
            grammar.Lessons
                .OrderBy(lesson => lesson.Order)
                .ThenBy(lesson => lesson.Id)
                .Select(lesson => lesson.ToResponse())
                .ToArray());
    }

    public static GrammarLessonResponse ToResponse(this GrammarLesson lesson)
    {
        return new GrammarLessonResponse(
            lesson.Id,
            lesson.Title,
            lesson.HtmlTag,
            lesson.Content,
            lesson.Order,
            lesson.Language,
            lesson.VoiceId);
    }
}
