using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Learnup.Application.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Learnup.Infrastructure.ExternalService;

public class AiService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AiService> logger) : IAiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };


    public async Task<T?> SendAsync<T>(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await SendAsync(messages, cancellationToken);
            try
            {
                return JsonSerializer.Deserialize<T>(response.Trim("```").Trim("json"), JsonOptions);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error deserializing ai message {Response}", response);
                return default;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error requesting ai message {Request}", messages.FirstOrDefault().Content);
            return default;
        }
    }

    public async Task<string> SendAsync(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default)
    {
        var completion = await CompleteAsync(messages, cancellationToken);

        return completion.Content;
    }

    public async Task<AiCompletion> CompleteAsync(IEnumerable<AiProxyMessage> messages, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient();
        var request = CreateRequest(messages, stream: false);

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        var content = doc.RootElement
                          .GetProperty("choices")[0]
                          .GetProperty("message")
                          .GetProperty("content")
                          .GetString()
                      ?? string.Empty;

        var usage = ParseUsage(doc.RootElement);

        return new AiCompletion(content, usage);
    }

    public async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        IEnumerable<AiProxyMessage> messages,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient();
        var request = CreateRequest(messages, stream: true);

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:", StringComparison.Ordinal))
            {
                continue;
            }

            var payload = line["data:".Length..].Trim();

            if (payload == "[DONE]")
            {
                break;
            }

            var chunk = ParseStreamChunk(payload);

            if (chunk is not null)
            {
                yield return chunk;
            }
        }
    }

    private HttpRequestMessage CreateRequest(IEnumerable<AiProxyMessage> messages, bool stream)
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}/chat/completions");
        request.Headers.Authorization = CreateAuthorizationHeader(apiKey);

        var mappedMessages = messages.Select(message => new
        {
            role = message.Role,
            content = message.Content
        }).ToArray();

        object body = stream
            ? new
            {
                model = modelName,
                messages = mappedMessages,
                stream = true,
                stream_options = new { include_usage = true }
            }
            : new
            {
                model = modelName,
                messages = mappedMessages
            };

        request.Content = JsonContent.Create(body);

        return request;
    }

    private AiStreamChunk? ParseStreamChunk(string payload)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            string? contentDelta = null;

            if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var choice = choices[0];

                if (choice.TryGetProperty("delta", out var delta)
                    && delta.TryGetProperty("content", out var content)
                    && content.ValueKind == JsonValueKind.String)
                {
                    contentDelta = content.GetString();
                }
            }

            AiTokenUsage? usage = null;

            if (root.TryGetProperty("usage", out var usageElement) && usageElement.ValueKind == JsonValueKind.Object)
            {
                usage = ParseUsage(root);
            }

            if (contentDelta is null && usage is null)
            {
                return null;
            }

            return new AiStreamChunk(contentDelta, usage);
        }
        catch (JsonException e)
        {
            logger.LogWarning(e, "Failed to parse AI stream chunk: {Payload}", payload);
            return null;
        }
    }

    private static AiTokenUsage ParseUsage(JsonElement root)
    {
        if (!root.TryGetProperty("usage", out var usage) || usage.ValueKind != JsonValueKind.Object)
        {
            return AiTokenUsage.Empty;
        }

        var promptTokens = GetInt(usage, "prompt_tokens");
        var completionTokens = GetInt(usage, "completion_tokens");
        var totalTokens = usage.TryGetProperty("total_tokens", out _)
            ? GetInt(usage, "total_tokens")
            : promptTokens + completionTokens;

        return new AiTokenUsage(promptTokens, completionTokens, totalTokens);
    }

    private static int GetInt(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var value) && value.TryGetInt32(out var number)
            ? number
            : 0;
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
