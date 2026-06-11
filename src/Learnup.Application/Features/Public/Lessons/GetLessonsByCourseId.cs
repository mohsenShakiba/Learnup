using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;

public sealed record GetLessonsByCourseId(int CourseId) : IRequest<IReadOnlyList<LessonResponse>>;

internal sealed class GetLessonsByCourseIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetLessonsByCourseId, IReadOnlyList<LessonResponse>>
{
    public async Task<IReadOnlyList<LessonResponse>> Handle(
        GetLessonsByCourseId request,
        CancellationToken cancellationToken)
    {
        var lessons = await dbContext.Lessons
            .AsNoTracking()
            .Include(l => l.Stories)
            .Include(l => l.Grammars)
            .Include(l => l.Vocabs)
            .Where(l => l.CourseId == request.CourseId)
            .OrderBy(l => l.Order)
            .ThenBy(l => l.Id)
            .ToListAsync(cancellationToken);

        var storyIds = lessons.SelectMany(l => l.Stories.Select(ls => ls.StoryId)).ToHashSet();
        var grammarIds = lessons.SelectMany(l => l.Grammars.Select(lg => lg.GrammarId)).ToHashSet();
        var vocabIds = lessons.SelectMany(l => l.Vocabs.Select(lv => lv.VocabId)).ToHashSet();

        var completedStoryIds = await dbContext.UserStories
            .AsNoTracking()
            .Where(us => us.UserId == identityProvider.UserId && us.CompletedAt != null && storyIds.Contains(us.StoryId))
            .Select(us => us.StoryId)
            .ToHashSetAsync(cancellationToken);

        var completedGrammarIds = await dbContext.UserGrammars
            .AsNoTracking()
            .Where(ug => ug.UserId == identityProvider.UserId && ug.CompletedAt != null && grammarIds.Contains(ug.GrammarId))
            .Select(ug => ug.GrammarId)
            .ToHashSetAsync(cancellationToken);

        var completedVocabIds = await dbContext.UserVocabs
            .AsNoTracking()
            .Where(uv => uv.UserId == identityProvider.UserId && uv.CompletedAt != null && vocabIds.Contains(uv.VocabId))
            .Select(uv => uv.VocabId)
            .ToHashSetAsync(cancellationToken);

        return lessons
            .Select(l => l.ToResponse(completedStoryIds, completedGrammarIds, completedVocabIds))
            .ToList();
    }
}
