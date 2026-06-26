using System.Diagnostics;
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
            var sw = Stopwatch.StartNew();
            
            foreach (var processor in processors)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                if (!processor.Enabled)
                {
                    continue;
                }

                try
                {
                    await processor.ProcessAsync(stoppingToken);
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "AI processor {ProcessorType} failed.",
                        processor.GetType().Name);
                }
            }

            if (sw.ElapsedMilliseconds < 1000)
            {
                await Task.Delay(1000 - (int)sw.ElapsedMilliseconds , stoppingToken);
            }
            
        }
    }
}
