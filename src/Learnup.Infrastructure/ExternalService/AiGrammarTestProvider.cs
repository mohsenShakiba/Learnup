using System.ClientModel;
using System.Text.Json;
using Learnup.Application.ExternalServices;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Stories;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Infrastructure.Prompts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;

namespace Learnup.Infrastructure.ExternalService;

public class AiGrammarTestProvider(IConfiguration configuration, ILogger<AiGrammarTestProvider> logger) : IGrammarTestProvider
{
    public async Task<List<TestGenerationResult>> GetGrammarTestAsync(Grammar grammar, Story story,
        CancellationToken cancellationToken = default)
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
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(baseUrl),
                    NetworkTimeout = TimeSpan.FromHours(1)
                });

            var chatClient = client.GetChatClient(modelName);
            var userMessage = "Grammar: " + grammar.Name + ", Description: " + grammar.Description;
            var systemMessage = "Story: " + string.Join('\n', story.Items.Select(i => i.Content));

            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    ChatMessage.CreateSystemMessage(GrammarTestPrompt.GetPrompt()),
                    ChatMessage.CreateSystemMessage(systemMessage),
                    ChatMessage.CreateUserMessage(userMessage)
                ],
                cancellationToken: cancellationToken);

            var text = completion.Content[0].Text;

            var result = JsonSerializer.Deserialize<List<TestResponse>>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Select(r =>
            {
                return new TestGenerationResult(
                    VocabTestType.FillInTheBlanks,
                    r.Question,
                    r.Options.Select(o => new TestOptionResult(o.Text, o.IsCorrect)).ToArray());
            }).ToList() ?? [];
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error generating grammar test for {GrammarName}", grammar.Name);
            throw;
        }
    }

    private record TestResponse(string Question, OptionResponse[] Options);

    private record OptionResponse(string Text, bool IsCorrect);
}