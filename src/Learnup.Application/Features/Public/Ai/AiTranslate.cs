using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Ai;

namespace Learnup.Application.Features.Public.Ai;

public sealed record AiTranslate(string Word, string Sentence) : IRequest<SendAiTextResponse>;

internal sealed class AiTranslateHandler(IAiTextService aiTextService)
    : IRequestHandler<AiTranslate, SendAiTextResponse>
{
    public async Task<SendAiTextResponse> Handle(AiTranslate request, CancellationToken cancellationToken)
    {
        return await aiTextService.SendAsync(request.Word, request.Sentence, cancellationToken);
    }
}
