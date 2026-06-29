using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Learnup.Application.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Learnup.Infrastructure.ExternalService;

public class KokoroVoiceProvider(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<KokoroVoiceProvider> logger,
    IFileService fileService)
    : IVoiceProvider
{
    public async Task<VoiceResult> GetVoiceAsync(string content, VoiceOptions? options, CancellationToken cancellationToken = default)
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
                voice = options?.VoiceId ?? "af_bella",
                speed = options?.PlaybackSpeed ?? 1.0,
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
            var fileName = $"kokoro-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}.wav";
            await using var audioStream = new MemoryStream(audioBytes);

            var fileId = await fileService.StoreAsync(new StoreFileRequest(
                audioStream,
                fileName,
                BucketNames.StoryVoices,
                "audio/wav"), cancellationToken);

            return new VoiceResult(fileId);
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
