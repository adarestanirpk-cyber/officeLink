using Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Workers;

public class OutboxProcessor: BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceProvider services, ILogger<OutboxProcessor> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var messages = await outboxService.GetUnprocessedMessagesAsync();

            foreach (var msg in messages)
            {
                try
                {
                    var type = Type.GetType($"Application.DTOs.{msg.Type}");
                    if (type == null)
                    {
                        _logger.LogWarning("Unknown message type {Type}", msg.Type);
                        continue;
                    }

                    var obj = JsonSerializer.Deserialize(msg.Data, type);
                    await publishEndpoint.Publish(obj!, stoppingToken);

                    await outboxService.MarkAsProcessedAsync(msg.Id);
                    _logger.LogInformation("Outbox message {Id} processed", msg.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Outbox message {Id}", msg.Id);
                    // 🚨 اینجا میشه Incident + Retry رو آپدیت کرد
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
