namespace Learnup.Application.ExternalServices;

public interface IVoiceProvider
{
    Task<VoiceResult> GetVoiceAsync(string content, CancellationToken cancellationToken = default);
}

public record VoiceResult(string VoiceId, List<VoiceCaption> Captions);
public record VoiceCaption(string Word, float Start, float End);