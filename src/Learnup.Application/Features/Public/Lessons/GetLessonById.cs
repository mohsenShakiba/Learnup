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

public sealed record GetLessonById(int Id) : IRequest<LessonDetailResponse?>;

internal sealed class GetLessonByIdHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<GetLessonById, LessonDetailResponse?>
{
    public async Task<LessonDetailResponse?> Handle(GetLessonById request, CancellationToken cancellationToken)
    {
        var lesson = await dbContext.Lessons
            .AsNoTracking()
            .Include(l => l.Stories).ThenInclude(ls => ls.Story).ThenInclude(s => s.Items)
            .Include(l => l.Grammars).ThenInclude(lg => lg.Grammar)
            .Include(l => l.Vocabs).ThenInclude(lv => lv.Vocab)
            .Include(l => l.Tests).ThenInclude(t => t.Options)
            .Include(l => l.Tests).ThenInclude(t => t.UserTestResults)
            .Where(l => l.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            return null;
        }

        var userLesson = await dbContext.UserLessons
            .FirstOrDefaultAsync(userLesson => userLesson.UserId == identityProvider.UserId
                                               && userLesson.LessonId == request.Id, cancellationToken);

        if (userLesson is null)
        {
            userLesson = new UserLesson(identityProvider.UserId, request.Id);
            userLesson.SetRequirements(lesson.Stories.Count, lesson.Grammars.Count, lesson.Vocabs.Count);
            dbContext.UserLessons.Add(userLesson);
        }
        else
        {
            userLesson.Touch();
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return lesson.ToDetailResponse();
    }
}