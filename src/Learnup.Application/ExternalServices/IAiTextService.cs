using Learnup.Application.Responses.Public.Ai;

namespace Learnup.Application.ExternalServices;

public interface IAiTextService
{
    Task<SendAiTextResponse> SendAsync(string word, string sentence, CancellationToken cancellationToken = default);
}
