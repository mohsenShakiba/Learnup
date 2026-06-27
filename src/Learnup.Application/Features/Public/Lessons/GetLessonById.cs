using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
using Learnup.Application.Responses.Public.Vocabs;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;

public sealed record GetLessonById(
    int Id,
    UserLessonEntityType? LastReadEntityType = null,
    int? LastReadEntityId = null) : IRequest<LessonDetailResponse?>;

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
            .Include(l => l.Vocabs).ThenInclude(lv => lv.Vocab)
            .Where(l => l.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            return null;
        }

        var testsCount = await dbContext.Tests
            .CountAsync(t => t.LessonId == lesson.Id && t.Type == TestType.Vocab, cancellationToken);

        var userTests = await dbContext.UserTestResults
            .Where(t => t.UserId == identityProvider.UserId
                && t.Test.LessonId == lesson.Id
                && t.Test.Type == TestType.Vocab)
            .Select(t => t.IsCorrect)
            .ToListAsync(cancellationToken);

        var test = new LessonTestResponse
        {
            IsPassed = testsCount == userTests.Count,
            Score = userTests.Count == 0 || testsCount == 0 ? 0 : userTests.Count / (float)testsCount * 100
        };

        var userLesson = await dbContext.UserLessons
            .FirstOrDefaultAsync(
                userLesson => userLesson.UserId == identityProvider.UserId
                    && userLesson.LessonId == request.Id,
                cancellationToken);

        if (userLesson is null)
        {
            userLesson = new UserLesson(identityProvider.UserId, request.Id);
            dbContext.UserLessons.Add(userLesson);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return lesson.ToDetailResponse(test);
    }
}
