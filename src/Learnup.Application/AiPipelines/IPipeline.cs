namespace Learnup.Application.AiPipelines;

public interface IPipeline
{
    Task ProcessAsync(CancellationToken cancellationToken = default);
}