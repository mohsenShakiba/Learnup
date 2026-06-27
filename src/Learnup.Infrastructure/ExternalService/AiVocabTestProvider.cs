using System.ClientModel;
using System.Text.Json;
using Learnup.Application.ExternalServices;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Prompts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;

namespace Learnup.Infrastructure.ExternalService;

public class AiVocabTestProvider(IConfiguration configuration, ILogger<AiVocabTestProvider> logger) : IVocabTestProvider
{
    public async Task<TestGenerationResult> GetVocabTestAsync(Vocab vocab, CancellationToken cancellationToken = default)
    {
        try
        {
            var credential = configuration["ApiKeys:LmStudioApiKey"];
            var baseUrl = configuration["ConnectionStrings:LmStudioUrl"];
            var modelName = configuration["ModelNames:LmStudioModelName"];

            if (string.IsNullOrWhiteSpace(credential) || string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("LmStudio configuration is missing");

            var client = new OpenAIClient(
                credential: new ApiKeyCredential(credential),
                options: new OpenAIClientOptions { Endpoint = new Uri(baseUrl) });

            var chatClient = client.GetChatClient(modelName);
            var userMessage = "Word: " + vocab.Word + ", Translation: " + vocab.Translation + ", type: " + vocab.Type;

            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    ChatMessage.CreateSystemMessage(VocabTestPrompt.GetPrompt()),
                    ChatMessage.CreateUserMessage(userMessage)
                ],
                cancellationToken: cancellationToken);
            
            var text = completion.Content[0].Text;

            var result = JsonSerializer.Deserialize<TestResponse>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
                throw new InvalidOperationException("Invalid response from LmStudio");

            return new TestGenerationResult(
                result.Type,
                result.Question,
                result.Options.Select(o => new TestOptionResult(o.Text, o.IsCorrect)).ToArray());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error generating vocab test for word {Word}", vocab.Word);
            throw;
        }
    }
    

    private record TestResponse(TestQuestionType Type, string Question, OptionResponse[] Options);
    private record OptionResponse(string Text, bool IsCorrect);
}
