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

public class AiTestProvider(IConfiguration configuration, ILogger<AiTestProvider> logger) : ITestProvider
{
    public async Task<TestGenerationResult[]> GenerateTestAsync(List<string> grammarTitles, List<string> vocabs,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var credential = configuration["OpenAiService:ApiKey"];
            var baseUrl = configuration["OpenAiService:BaseUrl"];
            var modelName = configuration["OpenAiService:ModelName"];

            if (string.IsNullOrWhiteSpace(credential) || string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("OpenAiService configuration is missing");

            var client = new OpenAIClient(
                credential: new ApiKeyCredential(credential),
                options: new OpenAIClientOptions { Endpoint = new Uri(baseUrl) });

            var chatClient = client.GetChatClient(modelName);
            var userMessage = "Words: " + string.Join(", ", vocabs) + "\nGrammars: " + string.Join(", ", grammarTitles);

            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    ChatMessage.CreateSystemMessage(VocabTestPrompt.GetPrompt()),
                    ChatMessage.CreateUserMessage(userMessage)
                ],
                cancellationToken: cancellationToken);

            var text = completion.Content[0].Text;

            var result = JsonSerializer.Deserialize<TestResponse[]>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
                throw new InvalidOperationException("Invalid response from LmStudio");

            return result.Select(r =>
            {
                return new TestGenerationResult(
                    r.Type,
                    r.Question,
                    r.Options.Select(o => new TestOptionResult(o.Text, o.IsCorrect)).ToArray());
            }).ToArray();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error generating vocab test for word");
            throw;
        }
    }


    private record TestResponse(TestQuestionType Type, string Question, OptionResponse[] Options);

    private record OptionResponse(string Text, bool IsCorrect);
}