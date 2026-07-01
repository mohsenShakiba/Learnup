using Learnup.Application.Authentication;
using Learnup.Application.Mappers;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
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
            .Include(l => l.Grammars).ThenInclude(lg => lg.Grammar).ThenInclude(g => g.Lessons)
            .Include(l => l.Vocabs).ThenInclude(lv => lv.Vocab).ThenInclude(v => v.Senses)
            .Include(l => l.Tests).ThenInclude(t => t.Options)
            .Include(l => l.Tests).ThenInclude(t => t.UserTestResults.Where(r => r.UserId == identityProvider.UserId))
            .Where(l => l.Id == request.Id)
            .AsSplitQuery()
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
            userLesson.SetRequirements(lesson.Stories.Count, lesson.Grammars.Count, lesson.Vocabs.Count, lesson.Tests.Count);
            dbContext.UserLessons.Add(userLesson);
        }
        else
        {
            userLesson.Touch();
        }

        var nextLessonId = await dbContext.Lessons
            .Where(l => l.CourseId == lesson.CourseId && l.Order > lesson.Order)
            .OrderBy(l => l.Order)
            .Select(l => l.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var lessonVocabIds = lesson.Vocabs.Select(lessonVocab => lessonVocab.VocabId).ToList();
        var leitnerVocabIds = await dbContext.LeitnerBoxes
            .AsNoTracking()
            .Where(box => box.UserId == identityProvider.UserId)
            .SelectMany(box => box.Items)
            .Where(item => lessonVocabIds.Contains(item.VocabId))
            .Select(item => item.VocabId)
            .Distinct()
            .ToListAsync(cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return lesson.ToDetailResponse(nextLessonId, leitnerVocabIds.ToHashSet());
    }
}
