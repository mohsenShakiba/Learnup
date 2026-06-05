using Learnup.Application.Requests.Admin.Stories;

namespace Learnup.Application.ExternalServices;

public interface IStoryLoader
{
    Task<int> LoadAsync(
        StoryRequest storyRequest,
        int courseId,
        int lessonId,
        List<int> grammarIds,
        CancellationToken cancellationToken = default);
}
