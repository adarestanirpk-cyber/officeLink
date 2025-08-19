using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class RetryWorker:BackgroundService
{
    private readonly ILogger<RetryWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RetryWorker(ILogger<RetryWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IRetryHandler>();

                var failedItems = await handler.GetFailedItemsAsync(stoppingToken);

                foreach (var item in failedItems)
                {
                    _logger.LogInformation("Retrying WFCaseLink {Id}", item.Id);

                    bool success = await handler.ProcessItemAsync(item, stoppingToken);
                    await handler.MarkAsRetriedAsync(item, success, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RetryWorker");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // ⏳ فاصله بین هر اسکن
        }
    }
}
