using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;

public enum StorySection
{
    Story = 1,
    Grammar = 2,
    GrammarTest = 3,
    Vocab = 4,
    VocabTest = 5
}

public record OnLessonSectionCompleted(int LessonId, StorySection Section) : IRequest;

public class OnLessonSectionCompletedHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider) : IRequestHandler<OnLessonSectionCompleted>
{
    public async Task<Unit> Handle(OnLessonSectionCompleted request, CancellationToken cancellationToken)
    {
        var userLesson = await dbContext.UserLessons
            .FirstOrDefaultAsync(
                ul => ul.UserId == identityProvider.UserId && ul.LessonId == request.LessonId,
                cancellationToken);

        if (request.Section == StorySection.Story)
        {
            userLesson?.CompleteStory();
        }
        
        if (request.Section == StorySection.Grammar)
        {
            userLesson?.CompleteGrammar();
        }
        
        if (request.Section == StorySection.GrammarTest)
        {
            userLesson?.CompleteGrammarTest();
        }
        
        if (request.Section == StorySection.Vocab)
        {
            userLesson?.CompleteVocab();
        }
        
        if (request.Section == StorySection.VocabTest)
        {
            userLesson?.CompleteVocabTest();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}