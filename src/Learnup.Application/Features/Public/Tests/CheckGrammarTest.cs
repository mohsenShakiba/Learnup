using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Tests;

public sealed record CheckGrammarTest(int LessonId) : IRequest;

internal sealed class CheckGrammarTestHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
    : IRequestHandler<CheckGrammarTest>
{
    public async Task<Unit> Handle(CheckGrammarTest request, CancellationToken cancellationToken)
    {
        var grammarIds = await dbContext.LessonGrammars
            .Where(lg => lg.LessonId == request.LessonId)
            .Select(lg => lg.GrammarId)
            .ToListAsync(cancellationToken);

        var testIds = await dbContext.GrammarTests
            .Where(t => grammarIds.Contains(t.GrammarId) && t.Status == TestStatus.Published)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        var answeredTestIds = await dbContext.UserGrammarTestResults
            .Where(r => r.UserId == identityProvider.UserId && testIds.Contains(r.GrammarTestId))
            .Select(r => r.GrammarTestId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (answeredTestIds.Count != testIds.Count)
        {
            throw new InvalidOperationException("All grammar test questions must be answered before checking the test");
        }

        var userGrammars = await dbContext.UserGrammars
            .Where(ug => ug.UserId == identityProvider.UserId && grammarIds.Contains(ug.GrammarId))
            .ToListAsync(cancellationToken);

        foreach (var grammarId in grammarIds)
        {
            var userGrammar = userGrammars.FirstOrDefault(ug => ug.GrammarId == grammarId);
            if (userGrammar is null)
            {
                userGrammar = new UserGrammar(identityProvider.UserId, grammarId);
                dbContext.UserGrammars.Add(userGrammar);
            }

            userGrammar.Complete();
        }

        var userLesson = await dbContext.UserLessons
            .FirstOrDefaultAsync(
                ul => ul.UserId == identityProvider.UserId && ul.LessonId == request.LessonId,
                cancellationToken);

        if (userLesson is null)
        {
            userLesson = new UserLesson(identityProvider.UserId, request.LessonId);
            dbContext.UserLessons.Add(userLesson);
        }
        else
        {
            userLesson.Touch();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
