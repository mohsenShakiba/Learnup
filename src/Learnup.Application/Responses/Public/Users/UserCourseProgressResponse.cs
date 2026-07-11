using Learnup.Application.Responses.Public.Grammars;
using Learnup.Application.Responses.Public.Conversations;
using Learnup.Application.Responses.Public.Vocabs;

namespace Learnup.Application.Responses.Public.Users;

public sealed record UserCourseProgressResponse(
    IReadOnlyList<GrammarResponse> Grammars,
    IReadOnlyList<ConversationResponse> Conversations);
