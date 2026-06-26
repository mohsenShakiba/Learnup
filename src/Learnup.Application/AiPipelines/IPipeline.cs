namespace Learnup.Application.AiPipelines;

public interface IPipeline
{
    public bool Enabled { get; }
    Task ProcessAsync(CancellationToken cancellationToken = default);
}