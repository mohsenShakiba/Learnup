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
            .Include(l => l.Stories).ThenInclude(ls => ls.Story).ThenInclude(s => s.Items).ThenInclude(i => i.Timestamps)
            .Include(l => l.Grammars).ThenInclude(lg => lg.Grammar)
            .Include(l => l.Vocabs).ThenInclude(lv => lv.Vocab)
            .Where(l => l.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            return null;
        }

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

        return lesson.ToDetailResponse();
    }
}
