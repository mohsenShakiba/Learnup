using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Ai;

namespace Learnup.Application.Features.Public.Ai;

public sealed record SendAiText(string Word, string Sentence) : IRequest<SendAiTextResponse>;

internal sealed class SendAiTextHandler(IAiTextService aiTextService)
    : IRequestHandler<SendAiText, SendAiTextResponse>
{
    public async Task<SendAiTextResponse> Handle(SendAiText request, CancellationToken cancellationToken)
    {
        return await aiTextService.SendAsync(request.Word, request.Sentence, cancellationToken);
    }
}
