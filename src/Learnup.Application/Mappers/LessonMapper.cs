using Learnup.Application.Mappings;
using Learnup.Application.Responses.Public.Grammars;
using Learnup.Application.Responses.Public.Stories;
using Learnup.Application.Responses.Public.Vocabs;
using Learnup.Application.Responses.Public.Lessons;
using Learnup.Domain.AggregateRoots.Lessons;

namespace Learnup.Application.Mappers;

public static class LessonMapper
{
    public static LessonResponse ToResponse(this Lesson lesson) =>
        new(lesson.Id, lesson.Title, lesson.Order, lesson.CourseId);

    public static LessonDetailResponse ToDetailResponse(this Lesson lesson) =>
        new(
            lesson.Id,
            lesson.Title,
            lesson.Order,
            lesson.CourseId,
            lesson.Stories.Select(ls => ls.Story.ToResponse()).ToList(),
            lesson.Grammars.Select(lg => lg.Grammar.ToResponse()).ToList(),
            lesson.Vocabs.Select(lv => lv.Vocab.ToResponse()).ToList());

    private static StoryResponse ToResponse(this Domain.AggregateRoots.Stories.Story story) =>
        new(
            story.Id,
            story.Title,
            story.CoverId,
            story.Items.Select(i => i.ToResponse()).ToList());

    private static StoryItemResponse ToResponse(this Domain.AggregateRoots.Stories.StoryItem item) =>
        new(
            item.Id,
            item.Content,
            item.Translation,
            item.Order,
            item.VoiceId,
            item.Timestamps.Select(t => new StoryItemTimestampResponse(t.Id, t.Word, t.Start, t.End)).ToList());

    private static VocabResponse ToResponse(this Domain.AggregateRoots.Vocabularies.Vocab vocab) =>
        new(vocab.Id, vocab.Word, vocab.Translation, vocab.VoiceId, vocab.Description, vocab.Level, vocab.ParentVocabId, vocab.LanguageId);
}
