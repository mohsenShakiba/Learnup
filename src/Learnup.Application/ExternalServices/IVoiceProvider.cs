namespace Learnup.Application.ExternalServices;

public interface IVoiceProvider
{
    Task<VoiceResult> GetVoiceAsync(string content, VoiceOptions? options = null, CancellationToken cancellationToken = default);

    Task<ConversationVoiceResult> GetConversationVoiceAsync(
        string text,
        VoiceOptions? options = null,
        CancellationToken cancellationToken = default);
}

public record VoiceOptions(string VoiceId, double PlaybackSpeed = 1);

/// <param name="VoiceId">Stored audio file id.</param>
/// <param name="Sentences">Sentence-level timings within the audio, when the provider supports them.</param>
public record VoiceResult(string VoiceId, IReadOnlyList<VoiceSentence>? Sentences = null);

/// <summary>A sentence and its start/end offset (seconds) within the generated audio.</summary>
public record VoiceSentence(string Text, double Start, double End);

/// <param name="AudioFileId">Stored audio file id for the whole conversation.</param>
/// <param name="TimestampsFileId">Stored JSON file id (sitting next to the audio) holding word-by-word timings.</param>
/// <param name="Words">Word-by-word timings within the audio.</param>
public record ConversationVoiceResult(
    string AudioFileId,
    string TimestampsFileId,
    IReadOnlyList<VoiceWord> Words);

/// <summary>A word and its start/end offset (seconds) within the generated audio.</summary>
public record VoiceWord(string Text, double Start, double End);

public static class VoiceIds
{
    public const string Bella = "af_bella";
    public const string Heart = "af_heart";
    public const string Echo = "am_echo";
}

// Free-plan-usable "premade" voices. Library/professional voices (e.g. Elise, Clara)
// return HTTP 402 on the free plan and require a paid subscription.
public static class ElevenLabsVoiceIds
{
    public const string George = "JBFqnCBsd6RMkjVDRZzb"; // male, warm storyteller
    public const string Brian = "nPczCjzI2devNBz1zQrb";  // male, deep, comforting
    public const string Sarah = "EXAVITQu4vr4xnSDxMaL";  // female, mature, reassuring
    public const string Alice = "Xb7hH8MSUJpSbSDYk0k2";  // female, clear educator
    public const string Lily = "pFZP5JQG7iQjIQuC4Bku";  // female, velvety actress 
}