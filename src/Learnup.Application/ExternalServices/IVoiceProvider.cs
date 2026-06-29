namespace Learnup.Application.ExternalServices;

public interface IVoiceProvider
{
    Task<VoiceResult> GetVoiceAsync(string content, VoiceOptions? options = null, CancellationToken cancellationToken = default);
}

public record VoiceOptions(string VoiceId, double PlaybackSpeed = 1);
public record VoiceResult(string VoiceId);

public static class VoiceIds
{
    public const string Bella = "af_bella";
    public const string Heart = "af_heart";
}