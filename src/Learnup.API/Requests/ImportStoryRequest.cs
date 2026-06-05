using Learnup.Application.Requests.Admin.Stories;

namespace Learnup.API.Requests;

public record ImportStoryRequest(
    int CourseId,
    int LessonId,
    List<int> GrammarIds,
    StoryRequest Story);
