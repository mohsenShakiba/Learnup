using Learnup.Application.Mappings;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Grammars;
using Learnup.Application.Responses.Public.Users;
using Learnup.Application.Responses.Public.Vocabs;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Users;

public sealed record GetUserCourseProgress(int UserId, int CourseId)
    : IRequest<UserCourseProgressResponse>;

internal sealed class GetUserCourseProgressHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetUserCourseProgress, UserCourseProgressResponse>
{
    public async Task<UserCourseProgressResponse> Handle(
        GetUserCourseProgress request,
        CancellationToken cancellationToken)
    {
        var courseLessons = dbContext.Lessons
            .Where(lesson => lesson.CourseId == request.CourseId);

        var grammarIds = courseLessons
            .SelectMany(lesson => lesson.Grammars)
            .Select(lessonGrammar => lessonGrammar.GrammarId);

        var storyIds = courseLessons
            .SelectMany(lesson => lesson.Stories)
            .Select(lessonStory => lessonStory.StoryId);

        var vocabIds = courseLessons
            .SelectMany(lesson => lesson.Vocabs)
            .Select(lessonVocab => lessonVocab.VocabId);

        var grammars = await dbContext.UserGrammars
            .AsNoTracking()
            .Where(userGrammar => userGrammar.UserId == request.UserId
                                  && userGrammar.CompletedAt != null
                                  && grammarIds.Contains(userGrammar.GrammarId))
            .Select(userGrammar => new GrammarResponse(
                userGrammar.Grammar.Id,
                userGrammar.Grammar.Name))
            .ToListAsync(cancellationToken);

        var vocabs = await dbContext.UserVocabs
            .AsNoTracking()
            .Where(userVocab => userVocab.UserId == request.UserId
                                && userVocab.CompletedAt != null
                                && vocabIds.Contains(userVocab.VocabId))
            .Select(userVocab => new VocabResponse(
                userVocab.Vocab.Id,
                userVocab.Vocab.Word,
                userVocab.Vocab.Translation,
                userVocab.Vocab.VoiceId,
                userVocab.Vocab.Description,
                userVocab.Vocab.Level,
                userVocab.Vocab.ParentVocabId,
                userVocab.Vocab.LanguageId))
            .ToListAsync(cancellationToken);

        var completedStories = await dbContext.UserStories
            .AsNoTracking()
            .Where(userStory => userStory.UserId == request.UserId
                                && userStory.CompletedAt != null
                                && storyIds.Contains(userStory.StoryId))
            .Include(userStory => userStory.Story)
            .ThenInclude(story => story.Items)
            .ThenInclude(item => item.Timestamps)
            .Select(userStory => userStory.Story)
            .ToListAsync(cancellationToken);

        var stories = completedStories
            .Select(story => story.ToResponse())
            .ToList();

        return new UserCourseProgressResponse(grammars, stories, vocabs);
    }
}