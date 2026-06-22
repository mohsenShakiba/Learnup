using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;

namespace Learnup.Application.Features.Public.Ai;

public sealed record SendAiText(string Prompt) : IRequest<string>;

internal sealed class SendAiTextHandler(IAiTextService aiTextService)
    : IRequestHandler<SendAiText, string>
{
    public async Task<string> Handle(SendAiText request, CancellationToken cancellationToken)
    {
        return await aiTextService.SendAsync(request.Prompt, cancellationToken);
    }
}
