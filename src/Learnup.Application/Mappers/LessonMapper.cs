using Learnup.Application.Mappings;
using Learnup.Application.Responses.Public.Grammars;
using Learnup.Application.Responses.Public.Stories;
using Learnup.Application.Responses.Public.Vocabs;
using Learnup.Application.Responses.Public.Lessons;
using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Application.Mappers;

public static class LessonMapper
{
    public static LessonResponse ToResponse(
        this Lesson lesson,
        HashSet<int> completedStoryIds,
        HashSet<int> completedGrammarIds,
        HashSet<int> completedVocabIds) =>
        new(
            lesson.Id,
            lesson.Title,
            lesson.Order,
            lesson.CourseId,
            lesson.Stories.Count,
            lesson.Grammars.Count,
            lesson.Vocabs.Count,
            lesson.Stories.Count(ls => completedStoryIds.Contains(ls.StoryId)),
            lesson.Grammars.Count(lg => completedGrammarIds.Contains(lg.GrammarId)),
            lesson.Vocabs.Count(lv => completedVocabIds.Contains(lv.VocabId)));

    public static LessonDetailResponse ToDetailResponse(
        this Lesson lesson,
        LessonVocabTestResponse vocabTest)
    {
        return new(
            lesson.Id,
            lesson.Title,
            lesson.Order,
            lesson.CourseId,
            lesson.Stories.Select(ls => ls.Story.ToResponse()).ToList(),
            lesson.Grammars.Select(lg => lg.Grammar.ToResponse()).ToList(),
            lesson.Vocabs.Select(v => v.Vocab.ToResponse()).ToList(),
            vocabTest);
    }

    private static StoryItemResponse ToResponse(this Domain.AggregateRoots.Stories.StoryItem item) =>
        new(
            item.Id,
            item.Content,
            item.Translation,
            item.Order,
            item.Person,
            item.VoiceId,
            item.Timestamps.Select(t => new StoryItemTimestampResponse(t.Id, t.Word, t.Start, t.End)).ToList());
}
