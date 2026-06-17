using System.ClientModel;
using System.Text.Json;
using Learnup.Application.ExternalServices;
using Learnup.Infrastructure.Prompts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;

namespace Learnup.Infrastructure.ExternalService;

public class AiGrammarTestProvider(IConfiguration configuration, ILogger<AiGrammarTestProvider> logger) : IGrammarTestProvider
{
    public async Task<TestGenerationResult> GetGrammarTestAsync(string grammarName, string description, CancellationToken cancellationToken = default)
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
            var userMessage = "Grammar: " + grammarName + ", Description: " + description;

            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    ChatMessage.CreateSystemMessage(GrammarTestPrompt.GetPrompt()),
                    ChatMessage.CreateUserMessage(userMessage)
                ],
                cancellationToken: cancellationToken);

            var text = completion.Content[0].Text;
            var cleanedText = ExtractJson(text);

            var result = JsonSerializer.Deserialize<TestResponse>(cleanedText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
                throw new InvalidOperationException("Invalid response from LmStudio");

            return new TestGenerationResult(
                result.Question,
                result.Options.Select(o => new TestOptionResult(o.Text, o.IsCorrect)).ToArray());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error generating grammar test for {GrammarName}", grammarName);
            throw;
        }
    }

    private static string ExtractJson(string text)
    {
        var start = text.IndexOf("{");
        var end = text.LastIndexOf("}");
        return start >= 0 && end > start ? text[start..(end + 1)] : text.Trim();
    }

    private record TestResponse(string Question, OptionResponse[] Options);
    private record OptionResponse(string Text, bool IsCorrect);
}
