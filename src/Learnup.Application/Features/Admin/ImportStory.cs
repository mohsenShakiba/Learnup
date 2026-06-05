using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Requests.Admin.Stories;

namespace Learnup.Application.Features.Admin;

public sealed record ImportStory(
    StoryRequest Story,
    int CourseId,
    int LessonId,
    List<int> GrammarIds) : IRequest<int>;

internal sealed class ImportStoryHandler(IStoryLoader storyLoader) : IRequestHandler<ImportStory, int>
{
    public Task<int> Handle(ImportStory request, CancellationToken cancellationToken)
    {
        return storyLoader.LoadAsync(
            request.Story,
            request.CourseId,
            request.LessonId,
            request.GrammarIds,
            cancellationToken);
    }
}
