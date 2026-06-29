using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Requests.Admin.Stories;

namespace Learnup.Application.Features.Admin;

public sealed record ImportStory(
    StoryRequest Story,
    int CourseId,
    int LessonOrder) : IRequest<int>;

internal sealed class ImportStoryHandler(StoryLoader storyLoader) : IRequestHandler<ImportStory, int>
{
    public Task<int> Handle(ImportStory request, CancellationToken cancellationToken)
    {
        return storyLoader.LoadAsync(
            request.Story,
            request.CourseId,
            request.LessonOrder,
            cancellationToken);
    }
}
