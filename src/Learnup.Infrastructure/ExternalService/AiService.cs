using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Learnup.Application.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Learnup.Infrastructure.ExternalService;

public class AiService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AiService> logger) : IAiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };


    public async Task<T?> SendAsync<T>(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await SendAsync(messages, cancellationToken);
            
            return JsonSerializer.Deserialize<T>(response.Trim("```").Trim("json"), JsonOptions);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error generating vocab test for word");
            return default;
        }
    }

    public async Task<string> SendAsync(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default)
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
            messages = messages.Select(message => new
            {
                role = message.Role,
                content = message.Content
            }).ToArray()
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
                   .GetString()
               ?? string.Empty;
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
}