using Learnup.Application.Requests.Admin.Stories;

namespace Learnup.Application.ExternalServices;

public interface IStoryLoader
{
    Task<int> LoadAsync(
        StoryRequest storyRequest,
        int courseId,
        int lessonOrder,
        CancellationToken cancellationToken = default);
}
