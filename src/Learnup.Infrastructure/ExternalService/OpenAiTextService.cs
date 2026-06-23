using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Learnup.Application.ExternalServices;
using Learnup.Application.Responses.Public.Ai;
using Microsoft.Extensions.Configuration;

namespace Learnup.Infrastructure.ExternalService;

public class OpenAiTextService(IConfiguration configuration, IHttpClientFactory httpClientFactory) : IAiTextService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<SendAiTextResponse> SendAsync(
        string word,
        string sentence,
        CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["OpenAiService:ApiKey"];
        var baseUrl = configuration["OpenAiService:BaseUrl"];
        var modelName = configuration["OpenAiService:ModelName"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAiService:ApiKey is not configured.");
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("OpenAiService:BaseUrl is not configured.");
        if (string.IsNullOrWhiteSpace(modelName))
            throw new InvalidOperationException("OpenAiService:ModelName is not configured.");

        var client = httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}/chat/completions");
        request.Headers.Authorization = CreateAuthorizationHeader(apiKey);

        var body = new
        {
            model = modelName,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "Translate the provided English word and English sentence to Farsi. Return only valid JSON with this exact shape: { \"wordTranslation\": \"...\", \"sentenceTranslation\": \"...\" }."
                },
                new { role = "user", content = $"Word: {word}\nSentence: {sentence}" }
            }
        };

        request.Content = JsonContent.Create(body);

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
            return new SendAiTextResponse(string.Empty, string.Empty);

        var resultJson = ExtractJsonObject(content);

        return JsonSerializer.Deserialize<SendAiTextResponse>(resultJson, JsonOptions)
               ?? new SendAiTextResponse(string.Empty, string.Empty);
    }

    private static AuthenticationHeaderValue CreateAuthorizationHeader(string apiKey)
    {
        var trimmed = apiKey.Trim();
        var separatorIndex = trimmed.IndexOf(' ');

        if (separatorIndex > 0)
        {
            return new AuthenticationHeaderValue(
                trimmed[..separatorIndex],
                trimmed[(separatorIndex + 1)..]);
        }

        return new AuthenticationHeaderValue("Bearer", trimmed);
    }

    private static string ExtractJsonObject(string content)
    {
        var trimmed = content.Trim();
        var start = trimmed.IndexOf('{');
        var end = trimmed.LastIndexOf('}');

        return start >= 0 && end > start
            ? trimmed[start..(end + 1)]
            : trimmed;
    }
}
