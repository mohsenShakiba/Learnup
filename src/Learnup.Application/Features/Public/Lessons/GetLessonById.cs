using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
using Learnup.Application.Responses.Public.Vocabs;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;
    
public sealed record GetLessonById(int Id) : IRequest<LessonDetailResponse?>;

internal sealed class GetLessonByIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetLessonById, LessonDetailResponse?>
{
    public async Task<LessonDetailResponse?> Handle(
        GetLessonById request,
        CancellationToken cancellationToken)
    {
        var lesson = await dbContext.Lessons
            .AsNoTracking()
            .Include(l => l.Stories).ThenInclude(ls => ls.Story).ThenInclude(s => s.Items)
            .Include(l => l.Grammars).ThenInclude(lg => lg.Grammar)
            .Include(l => l.Vocabs).ThenInclude(lv => lv.Vocab).ThenInclude(v => v.Tests)
            .Where(l => l.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (lesson is null)
        {
            return null;
        }
        
        var vocabIds = lesson.Vocabs.Select(lv => lv.VocabId).ToList();
        var vocabTestsCount = lesson.Vocabs.SelectMany(lv => lv.Vocab.Tests).Count();
        
        var userVocabTests = await dbContext.UserVocabTestResults
            .Where(t => t.UserId == identityProvider.UserId && vocabIds.Contains(t.VocabTest.Vocab.Id))
            .Select(t => t.IsCorrect)
            .ToListAsync(cancellationToken);

        var vocabTest = new LessonVocabTestResponse
        {
            IsPassed = vocabTestsCount == userVocabTests.Count,
            Score = userVocabTests.Count == 0 ? 0 : userVocabTests.Count / (float)vocabTestsCount * 100
        };

        var userLessonExists = await dbContext.UserLessons
            .AsNoTracking()
            .AnyAsync(
                userLesson => userLesson.UserId == identityProvider.UserId
                    && userLesson.LessonId == request.Id,
                cancellationToken);

        if (!userLessonExists)
        {
            dbContext.UserLessons.Add(new UserLesson(identityProvider.UserId, request.Id));

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return lesson.ToDetailResponse(vocabTest);
    }
}
