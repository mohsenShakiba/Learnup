using System.Text.Json;
using Learnup.Application.Authentication;
using Learnup.Application.Exceptions;
using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Learnup.Application.Services;
using Learnup.Domain.AggregateRoots.Users;
using Learnup.Infrastructure.Prompts;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

public sealed record AiTranslate(string Word, string Sentence) : IRequest<SendAiTextResponse>;

internal sealed class AiTranslateHandler(
    IAiService aiService,
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<AiTranslate, SendAiTextResponse>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<SendAiTextResponse> Handle(AiTranslate request, CancellationToken cancellationToken)
    {
        var tokenUsage = await dbContext.UserTokenUsages
            .FirstOrDefaultAsync(entry => entry.UserId == identityProvider.UserId, cancellationToken);

        if (tokenUsage is null)
        {
            tokenUsage = new UserTokenUsage(identityProvider.UserId);
            dbContext.UserTokenUsages.Add(tokenUsage);
        }

        if (tokenUsage.TotalTokens >= tokenUsage.AvailableTokens)
        {
            throw new TokenUsageExceedException();
        }

        var completion = await aiService.CompleteAsync(
            [
                new AiProxyMessage("system", TranslationPrompt.GetPrompt()),
                new AiProxyMessage("user", $"""
                                            Word: {request.Word}
                                            Sentence: {request.Sentence}
                                            """)
            ],
            cancellationToken);

        var content = JsonSerializer.Deserialize<SendAiTextResponse>(
            NormalizeJsonContent(completion.Content),
            JsonOptions);

        if (content is null)
        {
            throw new InvalidOperationException("AI service returned null response.");
        }

        await ChatExecutionService.RecordTokenUsageAsync(
            dbContext,
            identityProvider.UserId,
            completion.Usage,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return content;
    }

    private static string NormalizeJsonContent(string content)
    {
        var normalized = content.Trim();

        if (normalized.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized["```json".Length..];
        }
        else if (normalized.StartsWith("```", StringComparison.Ordinal))
        {
            normalized = normalized["```".Length..];
        }

        if (normalized.EndsWith("```", StringComparison.Ordinal))
        {
            normalized = normalized[..^"```".Length];
        }

        return normalized.Trim();
    }
}
