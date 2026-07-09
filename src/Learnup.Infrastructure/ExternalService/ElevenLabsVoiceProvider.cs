using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
    : IVoiceProvider
{
    private const string DefaultBaseUrl = "https://api.elevenlabs.io";
    private const string ModelId = "eleven_v3";
    private static readonly char[] SentenceTerminators = ['.', '!', '?', '…'];

    private static readonly JsonSerializerOptions TimestampsJsonOptions = new()
    {
        WriteIndented = true,
    };

    public async Task<VoiceResult> GetVoiceAsync(string content, VoiceOptions? options, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await RequestTimestampsAsync(content, options, cancellationToken);

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

    public async Task<ConversationVoiceResult> GetConversationVoiceAsync(
        IReadOnlyList<VoiceTurn> turns,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await RequestDialogueTimestampsAsync(turns, cancellationToken);

            var audioBytes = Convert.FromBase64String(response.AudioBase64);

            // Shared base name so the audio and its timestamps JSON sit next to each other
            // in the same bucket (elevenlabs-...mp3 / elevenlabs-...json).
            var baseName = $"elevenlabs-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}";

            await using var audioStream = new MemoryStream(audioBytes);
            var audioFileId = await fileService.StoreAsync(new StoreFileRequest(
                audioStream,
                $"{baseName}.mp3",
                BucketNames.StoryVoices,
                "audio/mpeg"), cancellationToken);

            var words = BuildWords(response.Alignment, TurnBoundaries(response.VoiceSegments));

            var timestampsBytes = JsonSerializer.SerializeToUtf8Bytes(
                new
                {
                    words = words.Select(w => new { text = w.Text, start = w.Start, end = w.End }),
                },
                TimestampsJsonOptions);

            await using var timestampsStream = new MemoryStream(timestampsBytes);
            var timestampsFileId = await fileService.StoreAsync(new StoreFileRequest(
                timestampsStream,
                $"{baseName}.json",
                BucketNames.StoryVoices,
                "application/json"), cancellationToken);

            return new ConversationVoiceResult(audioFileId, timestampsFileId, words);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting conversation voice from ElevenLabs");
            throw;
        }
    }

    /// <summary>
    /// Calls ElevenLabs' text-to-speech "with timestamps" endpoint and returns the base64 audio
    /// together with the per-character alignment.
    /// </summary>
    private async Task<TimestampedResponse> RequestTimestampsAsync(
        string text,
        VoiceOptions? options,
        CancellationToken cancellationToken)
    {
        var voiceId = options?.VoiceId ?? ElevenLabsVoiceIds.Sarah;

        var client = CreateClient();

        var httpResponse = await client.PostAsJsonAsync(
            $"/v1/text-to-speech/{voiceId}/with-timestamps",
            new
            {
                text,
                model_id = ModelId,
                voice_settings = new
                {
                    stability = 0.5,
                    similarity_boost = 0.75,
                    speed = options?.PlaybackSpeed ?? 1.0,
                },
            },
            cancellationToken: cancellationToken);

        return await ReadTimestampedResponseAsync(
            httpResponse,
            $"voice '{voiceId}' (model '{ModelId}')",
            cancellationToken);
    }

    /// <summary>
    /// Calls ElevenLabs' text-to-dialogue "with timestamps" endpoint, which renders a multi-speaker
    /// conversation into a single audio track. Each turn carries its own voice id, so alternating
    /// turns can use different voices. Returns the base64 audio together with the per-character
    /// alignment (same shape as the single-voice endpoint).
    /// </summary>
    private async Task<TimestampedResponse> RequestDialogueTimestampsAsync(
        IReadOnlyList<VoiceTurn> turns,
        CancellationToken cancellationToken)
    {
        var client = CreateClient();

        var httpResponse = await client.PostAsJsonAsync(
            "/v1/text-to-dialogue/with-timestamps",
            new
            {
                inputs = turns.Select(t => new { text = t.Text, voice_id = t.VoiceId }),
                model_id = ModelId,
            },
            cancellationToken: cancellationToken);

        var voiceIds = string.Join(", ", turns.Select(t => t.VoiceId).Distinct());

        return await ReadTimestampedResponseAsync(
            httpResponse,
            $"voices '{voiceIds}' (model '{ModelId}')",
            cancellationToken);
    }

    private HttpClient CreateClient()
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

        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Add("xi-api-key", apiKey);

        return client;
    }

    private static async Task<TimestampedResponse> ReadTimestampedResponseAsync(
        HttpResponseMessage httpResponse,
        string requestDescription,
        CancellationToken cancellationToken)
    {
        using (httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException(
                    $"ElevenLabs returned {(int)httpResponse.StatusCode} {httpResponse.StatusCode} for {requestDescription}: {error}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<TimestampedResponse>(cancellationToken);

            if (response is null || string.IsNullOrEmpty(response.AudioBase64))
            {
                throw new InvalidOperationException("ElevenLabs returned an empty audio response");
            }

            return response;
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

    /// <summary>
    /// The character index (into the alignment) at which each dialogue turn after the first begins.
    /// The dialogue endpoint concatenates each turn's alignment with no separating whitespace, so
    /// these indices are where one speaker's last word would otherwise be glued to the next
    /// speaker's first word; <see cref="BuildWords"/> forces a word break at each of them.
    /// </summary>
    private static IReadOnlySet<int> TurnBoundaries(IReadOnlyList<VoiceSegment>? segments)
    {
        if (segments is not { Count: > 1 })
        {
            return new HashSet<int>();
        }

        // Skip the first segment: index 0 is not a boundary between two turns.
        return segments.Skip(1).Select(s => s.CharacterStartIndex).ToHashSet();
    }

    /// <summary>
    /// Groups ElevenLabs' per-character alignment into words. A word runs from its first
    /// non-whitespace character to the last character before the next whitespace; attached
    /// punctuation stays with the word. Its start/end are those characters' timings (seconds).
    /// A turn boundary (see <paramref name="turnBoundaries"/>) also ends the current word so that
    /// two adjacent speakers' words are never merged into one.
    /// </summary>
    private static IReadOnlyList<VoiceWord> BuildWords(Alignment? alignment, IReadOnlySet<int> turnBoundaries)
    {
        if (alignment?.Characters is not { Count: > 0 } characters
            || alignment.StartTimes is not { } starts
            || alignment.EndTimes is not { } ends)
        {
            return [];
        }

        var count = Math.Min(characters.Count, Math.Min(starts.Count, ends.Count));
        var words = new List<VoiceWord>();
        var text = new StringBuilder();
        double wordStart = 0;
        double wordEnd = 0;

        for (var i = 0; i < count; i++)
        {
            // A new turn starts here: flush the previous speaker's word before the new one begins.
            if (turnBoundaries.Contains(i))
            {
                AppendWord(words, text, wordStart, wordEnd);
            }

            // Each alignment entry is a single-character string (occasionally longer).
            var token = characters[i];
            if (string.IsNullOrEmpty(token))
            {
                continue;
            }

            // Whitespace ends the current word.
            if (string.IsNullOrWhiteSpace(token))
            {
                AppendWord(words, text, wordStart, wordEnd);
                continue;
            }

            if (text.Length == 0)
            {
                wordStart = starts[i];
            }

            text.Append(token);
            wordEnd = ends[i];
        }

        // Flush the final word (no trailing whitespace).
        AppendWord(words, text, wordStart, wordEnd);

        return words;
    }

    private static void AppendWord(List<VoiceWord> words, StringBuilder text, double start, double end)
    {
        if (text.Length == 0)
        {
            return;
        }

        words.Add(new VoiceWord(text.ToString(), start, end));
        text.Clear();
    }

    private record TimestampedResponse(
        [property: JsonPropertyName("audio_base64")]
        string AudioBase64,
        [property: JsonPropertyName("alignment")]
        Alignment? Alignment,
        [property: JsonPropertyName("voice_segments")]
        List<VoiceSegment>? VoiceSegments = null);

    private record VoiceSegment(
        [property: JsonPropertyName("character_start_index")]
        int CharacterStartIndex,
        [property: JsonPropertyName("character_end_index")]
        int CharacterEndIndex);

    private record Alignment(
        [property: JsonPropertyName("characters")]
        List<string>? Characters,
        [property: JsonPropertyName("character_start_times_seconds")]
        List<double>? StartTimes,
        [property: JsonPropertyName("character_end_times_seconds")]
        List<double>? EndTimes);
}
