using System.Net.Http.Json;
using System.Text.Json;
using Learnup.Application.ExternalServices;
using Microsoft.Extensions.Configuration;

namespace Learnup.Infrastructure.ExternalService;

public class OpenAiTextService(IConfiguration configuration, IHttpClientFactory httpClientFactory) : IAiTextService
{
    public async Task<string> SendAsync(string prompt, CancellationToken cancellationToken = default)
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
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var body = new
        {
            model = modelName,
            messages = new[]
            {
                new { role = "system", content = "Translate the following sentence to farsi, be concise." },
                new { role = "user", content = prompt }
            }
        };

        request.Content = JsonContent.Create(body);

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }
}
