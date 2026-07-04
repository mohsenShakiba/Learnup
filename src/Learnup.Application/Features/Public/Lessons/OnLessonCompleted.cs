using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;


public record OnLessonSectionCompleted(int LessonId, UserLessonStatus Status) : IRequest;

public class OnLessonSectionCompletedHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider) : IRequestHandler<OnLessonSectionCompleted>
{
    public async Task<Unit> Handle(OnLessonSectionCompleted request, CancellationToken cancellationToken)
    {
        var userLesson = await dbContext.UserLessons
            .FirstOrDefaultAsync(
                ul => ul.UserId == identityProvider.UserId && ul.LessonId == request.LessonId,
                cancellationToken);

        userLesson?.Complete(request.Status);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}