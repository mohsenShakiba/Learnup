using System.ClientModel;
using System.Text.Json;
using Learnup.Application.ExternalServices;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Prompts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;

namespace Learnup.Infrastructure.ExternalService;

public class AiVocabTranslationProvider(IConfiguration configuration, ILogger<AiVocabTranslationProvider> logger) : IVocabTranslationProvider
{

    public async Task<VocabTransactionResult> GetVocabTranslationAsync(
        string word,
        VocabType type,
        CancellationToken cancellationToken = default)
    {
        return null;
        // try
        // {
        //     var credential = configuration["ApiKeys:LmStudioApiKey"];
        //     var baseUrl = configuration["ConnectionStrings:LmStudioUrl"];
        //     var modelName = configuration["ModelNames:LmStudioModelName"];
        //
        //     if (string.IsNullOrWhiteSpace(credential) || string.IsNullOrWhiteSpace(baseUrl))
        //     {
        //         throw new InvalidOperationException("ApiKeys:LmStudioApiKey or ConnectionStrings:LmStudioUrl is not set");
        //     }
        //
        //     var client = new OpenAIClient(
        //         credential: new ApiKeyCredential(credential),
        //         options: new OpenAIClientOptions
        //         {
        //             Endpoint = new Uri(baseUrl)
        //         }
        //     );
        //
        //     var chatClient = client.GetChatClient(modelName);
        //
        //     ChatCompletion completion = await chatClient.CompleteChatAsync(
        //         [
        //             ChatMessage.CreateSystemMessage(VocabTranslationDetailPrompt.GetPrompt()),
        //             ChatMessage.CreateUserMessage($"Word: {word}")
        //         ],
        //         cancellationToken: cancellationToken
        //     );
        //
        //     var text = completion.Content[0].Text;
        //
        //     var cleanedText = text.Replace("```json", "").Replace("```", "").Trim();
        //     
        //     var result = JsonSerializer.Deserialize<VocabTransactionResult>(cleanedText);
        //
        //     return result ?? [];
        // }
        // catch (Exception e)
        // {
        //     logger.LogError(e, "Error getting vocab translation detail from LmStudio");
        //     throw;
        // }
    }
}
