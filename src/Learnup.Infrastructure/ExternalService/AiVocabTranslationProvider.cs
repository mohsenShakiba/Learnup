using System.ClientModel;
using System.Text.Json;
using Learnup.Application.ExternalServices;
using Learnup.Infrastructure.Prompts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;

namespace Learnup.Infrastructure.ExternalService;

public class AiVocabTranslationProvider(IConfiguration configuration, ILogger<AiVocabTranslationProvider> logger) : IVocabTranslationProvider
{
    public async Task<TranslationResult> GetTranslationAsync(string content, CancellationToken cancellationToken = default)
    {
        try
        {
            var credential = configuration["ApiKeys:LmStudioApiKey"];
            var baseUrl = configuration["ConnectionStrings:LmStudioUrl"];
            var modelName = configuration["ModelNames:LmStudioModelName"];

            if (string.IsNullOrWhiteSpace(credential) || string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("ApiKeys:LmStudioApiKey or ConnectionStrings:LmStudioUrl is not set");
            }

            var client = new OpenAIClient(
                credential: new ApiKeyCredential(credential),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(baseUrl)
                }
            );

            var chatClient = client.GetChatClient(modelName);

            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    ChatMessage.CreateSystemMessage(VocabTranslationPrompt.GetPrompt()),
                    ChatMessage.CreateUserMessage(content)
                ]
            );

            var text = completion.Content[0].Text;

            var cleanedText = text.Replace("```json", "").Replace("```", "").Trim();
            
            var result = JsonSerializer.Deserialize<Response>(cleanedText);

            if (result is null)
            {
                throw new InvalidOperationException("Invalid response from LmStudio");
            }

            return new TranslationResult(
                result.Translation,
                result.Description,
                result.ParentWord,
                result.Transactions ?? []);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting translation from LmStudio");
            throw;
        }
    }

    private record Response(
        string Translation,
        string? Description,
        string? ParentWord,
        List<VocabTransactionResult>? Transactions);
}
