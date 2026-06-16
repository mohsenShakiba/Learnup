using Learnup.Application.AiPipelines;

namespace Learnup.API.HostedServices;

public class AiProcessorHostedService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<AiProcessorHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var processors = scope.ServiceProvider.GetServices<IPipeline>().ToArray();

        while (stoppingToken.IsCancellationRequested == false)
        {
            foreach (var processor in processors)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    await processor.ProcessAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "AI processor {ProcessorType} failed.",
                        processor.GetType().Name);
                }
            }
        }
    }
}
