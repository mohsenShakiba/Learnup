using Learnup.Application.Mappings;
using Learnup.Application.Responses.Public.Grammars;
using Learnup.Application.Responses.Public.Stories;
using Learnup.Application.Responses.Public.Tests;
using Learnup.Application.Responses.Public.Vocabs;
using Learnup.Application.Responses.Public.Lessons;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;

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

    public static LessonDetailResponse ToDetailResponse(
        this Lesson lesson,
        int? nextLessonId,
        IReadOnlySet<int> leitnerVocabIds)
    {
        return new(
            lesson.Id,
            lesson.Title,
            lesson.Order,
            lesson.CourseId,
            nextLessonId,
            lesson.Users.FirstOrDefault()?.ToResponse() ?? new UserLessonResponse(UserLessonStatus.None, false, false, false, false, false),
            lesson.Stories.Select(s => s.Story.ToResponse()).ToList(),
            lesson.Grammars.Select(g => g.Grammar.ToResponse()).ToList(),
            lesson.Vocabs.Select(v => v.Vocab.ToResponse(leitnerVocabIds.Contains(v.VocabId))).ToList(),
            lesson.Tests.Select(t => t.ToResponse()).ToList());
    }

    public static UserLessonResponse ToResponse(this UserLesson userLesson)
    {
        return new(
            userLesson.Status,
            userLesson.IsStoryCompleted,
            userLesson.IsGrammarCompleted,
            userLesson.IsVocabCompleted,
            userLesson.IsGrammarTestCompleted,
            userLesson.IsVocabTestCompleted);
    }

    private static TestResponse ToResponse(this Test test)
    {
        var userTestResult = test.UserTestResults.FirstOrDefault();

        return new(
            test.Id,
            test.LessonId,
            test.Type,
            test.QuestionType,
            test.Question,
            test.Options
                .Select(o => new TestOptionResponse(o.Id, o.Text))
                .ToList(),
            userTestResult?.SelectedOptionId,
            userTestResult?.IsCorrect);
    }

    private static StoryItemResponse ToResponse(this Domain.AggregateRoots.Stories.StoryItem item) =>
        new(
            item.Id,
            item.Content,
            item.Translation,
            item.Order,
            item.Person,
            item.VoiceId);
}
