using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Learnup.Application.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Learnup.Infrastructure.ExternalService;

public class ElevenLabsVoiceProvider(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<ElevenLabsVoiceProvider> logger,
    IFileService fileService)
    : IElevenLabsVoiceProvider
{
    private const string DefaultBaseUrl = "https://api.elevenlabs.io";
    private const string ModelId = "eleven_v3";
    private static readonly char[] SentenceTerminators = ['.', '!', '?', '…'];

    public async Task<VoiceResult> GetVoiceAsync(string content, VoiceOptions? options, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKey = configuration.GetConnectionString("ElevenLabsApiKey")
                         ?? configuration["ElevenLabs:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("ElevenLabs API key is not set");
            }

            var baseUrl = configuration.GetConnectionString("ElevenLabsUrl")
                          ?? configuration["ElevenLabs:BaseUrl"]
                          ?? DefaultBaseUrl;

            var voiceId = options?.VoiceId ?? ElevenLabsVoiceIds.Sarah;

            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("xi-api-key", apiKey);

            using var httpResponse = await client.PostAsJsonAsync(
                $"/v1/text-to-speech/{voiceId}/with-timestamps",
                new
                {
                    text = content,
                    model_id = ModelId,
                    voice_settings = new
                    {
                        stability = 0.5,
                        similarity_boost = 0.75,
                        speed = options?.PlaybackSpeed ?? 1.0,
                    },
                },
                cancellationToken: cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException(
                    $"ElevenLabs returned {(int)httpResponse.StatusCode} {httpResponse.StatusCode} for voice '{voiceId}' (model '{ModelId}'): {error}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<TimestampedResponse>(cancellationToken);

            if (response is null || string.IsNullOrEmpty(response.AudioBase64))
            {
                throw new InvalidOperationException("ElevenLabs returned an empty audio response");
            }

            var audioBytes = Convert.FromBase64String(response.AudioBase64);
            var fileName = $"elevenlabs-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}.mp3";
            await using var audioStream = new MemoryStream(audioBytes);

            var fileId = await fileService.StoreAsync(new StoreFileRequest(
                audioStream,
                fileName,
                BucketNames.StoryVoices,
                "audio/mpeg"), cancellationToken);

            var sentences = BuildSentences(response.Alignment);

            return new VoiceResult(fileId, sentences);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting voice from ElevenLabs");
            throw;
        }
    }

    /// <summary>
    /// Groups ElevenLabs' per-character alignment into sentences. A sentence ends after a
    /// terminating punctuation character (.!?…); its start is the first character's start time
    /// and its end is the last character's end time.
    /// </summary>
    private static IReadOnlyList<VoiceSentence>? BuildSentences(Alignment? alignment)
    {
        if (alignment?.Characters is not { Count: > 0 } characters
            || alignment.StartTimes is not { } starts
            || alignment.EndTimes is not { } ends)
        {
            return null;
        }

        var count = Math.Min(characters.Count, Math.Min(starts.Count, ends.Count));
        var sentences = new List<VoiceSentence>();
        var text = new StringBuilder();
        double? sentenceStart = null;

        for (var i = 0; i < count; i++)
        {
            // Each alignment entry is a single-character string (occasionally longer).
            var token = characters[i];
            if (string.IsNullOrEmpty(token))
            {
                continue;
            }

            text.Append(token);

            // Capture the start time at the first non-whitespace character of the sentence.
            if (sentenceStart is null && !string.IsNullOrWhiteSpace(token))
            {
                sentenceStart = starts[i];
            }

            if (Array.IndexOf(SentenceTerminators, token[^1]) >= 0)
            {
                AppendSentence(sentences, text, sentenceStart, ends[i]);
                text.Clear();
                sentenceStart = null;
            }
        }

        // Flush any trailing text that did not end with terminating punctuation.
        if (text.Length > 0 && count > 0)
        {
            AppendSentence(sentences, text, sentenceStart, ends[count - 1]);
        }

        return sentences.Count > 0 ? sentences : null;
    }

    private static void AppendSentence(List<VoiceSentence> sentences, StringBuilder text, double? start, double end)
    {
        var trimmed = text.ToString().Trim();
        if (trimmed.Length == 0)
        {
            return;
        }

        sentences.Add(new VoiceSentence(trimmed, start ?? end, end));
    }

    private record TimestampedResponse(
        [property: JsonPropertyName("audio_base64")]
        string AudioBase64,
        [property: JsonPropertyName("alignment")]
        Alignment? Alignment);

    private record Alignment(
        [property: JsonPropertyName("characters")]
        List<string>? Characters,
        [property: JsonPropertyName("character_start_times_seconds")]
        List<double>? StartTimes,
        [property: JsonPropertyName("character_end_times_seconds")]
        List<double>? EndTimes);
}
