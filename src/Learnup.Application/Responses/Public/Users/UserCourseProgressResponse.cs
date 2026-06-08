using Learnup.Application.Responses.Public.Grammars;
using Learnup.Application.Responses.Public.Stories;
using Learnup.Application.Responses.Public.Vocabs;

namespace Learnup.Application.Responses.Public.Users;

public sealed record UserCourseProgressResponse(
    IReadOnlyList<GrammarResponse> Grammars,
    IReadOnlyList<StoryResponse> Stories,
    IReadOnlyList<VocabResponse> Vocabs);
