using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Learnup.Application.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Learnup.Infrastructure.ExternalService;

public class KokoroVoiceProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<KokoroVoiceProvider> logger)
    : IVoiceProvider
{
    public async Task<VoiceResult> GetVoiceAsync(string content, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = configuration.GetConnectionString("KokoroUrl");

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException("KokoroVoiceUrl or KokoroUrl is not set");
            }

            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(url);

            using var httpResponse = await client.PostAsJsonAsync("/dev/captioned_speech", new
            {
                model = "kokoro",
                input = content,
                voice = "af_heart",
                response_format = "wav",
            }, cancellationToken: cancellationToken);

            httpResponse.EnsureSuccessStatusCode();

            var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            var response = JsonSerializer.Deserialize<Response>(json);

            if (response is null)
            {
                throw new InvalidOperationException("Kokoro returned an empty response");
            }

            var audioBytes = Convert.FromBase64String(RemoveDataUriPrefix(response.Audio));
            var fileName = $"kokoro-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}.mp3";
            var outputDirectory = configuration["VoiceConfiguration:OutputDirectory"];

            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new InvalidOperationException("VoiceOutput is not set");
            }
            
            Directory.CreateDirectory(outputDirectory);

            var filePath = Path.Combine(outputDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, audioBytes, cancellationToken);

            var captions = response.Timestamps
                .Select(timestamp => new VoiceCaption(timestamp.Word, timestamp.StartTime, timestamp.EndTime))
                .ToList();

            return new VoiceResult(fileName, captions);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting voice from Kokoro");
            throw;
        }
    }

    private static string RemoveDataUriPrefix(string audio)
    {
        var commaIndex = audio.IndexOf(',');
        return commaIndex >= 0 ? audio[(commaIndex + 1)..] : audio;
    }

    private record Response(
        [property: JsonPropertyName("audio")] string Audio,
        [property: JsonPropertyName("timestamps")]
        List<Caption> Timestamps);

    private record Caption(
        [property: JsonPropertyName("word")] string Word,
        [property: JsonPropertyName("start_time")]
        float StartTime,
        [property: JsonPropertyName("end_time")]
        float EndTime);
}