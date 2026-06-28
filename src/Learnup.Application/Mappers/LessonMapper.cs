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
        this Lesson lesson)
    {
        var userLesson = lesson.Users.FirstOrDefault();

        return new(
            lesson.Id,
            lesson.Title,
            lesson.Order,
            lesson.CourseId,
            userLesson?.IsStoryCompleted ?? false,
            userLesson?.IsGrammarTestCompleted ?? false,
            userLesson?.IsVocabTestCompleted ?? false);
    }

    public static LessonDetailResponse ToDetailResponse(this Lesson lesson)
    {
        return new(
            lesson.Id,
            lesson.Title,
            lesson.Order,
            lesson.CourseId,
            lesson.Stories.Select(s => s.StoryId),
            lesson.Grammars.Select(s => s.GrammarId),
            lesson.Tests.Where(t => t.).Select(t => t.Id));
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
