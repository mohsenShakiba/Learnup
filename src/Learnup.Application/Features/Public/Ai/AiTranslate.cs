using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Ai;
using Learnup.Infrastructure.Prompts;

namespace Learnup.Application.Features.Public.Ai;

public sealed record AiTranslate(string Word, string Sentence) : IRequest<SendAiTextResponse>;

internal sealed class AiTranslateHandler(IAiService aiService)
    : IRequestHandler<AiTranslate, SendAiTextResponse>
{
    public async Task<SendAiTextResponse> Handle(AiTranslate request, CancellationToken cancellationToken)
    {
        var content = await aiService.SendAsync<SendAiTextResponse>(
            [
                new AiProxyMessage("system", TranslationPrompt.GetPrompt()),
                new AiProxyMessage("user", $"""
                                            Word: {request.Word}
                                            Sentence: {request.Sentence}
                                            """)
            ],
            cancellationToken);

        if (content is null)
        {
            throw new InvalidOperationException("AI service returned null response.");
        }

        return content;
    }
}