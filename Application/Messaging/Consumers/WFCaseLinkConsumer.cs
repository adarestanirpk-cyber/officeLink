using Application.Extensions;
using Application.Interfaces;
using Application.Messaging.Contract;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Messaging.Consumers;

public class WFCaseLinkConsumer : IConsumer<WFCaseLinkCreated>
{
    private readonly ILogger<WFCaseLinkConsumer> _logger;
    private readonly IWFCaseLinkService _service;
    public WFCaseLinkConsumer(ILogger<WFCaseLinkConsumer> logger, IWFCaseLinkService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task Consume(ConsumeContext<WFCaseLinkCreated> context)
    {
        var msg = context.Message;
        _logger.LogInformation(
            "Received message: {SourceCaseId} -> {TargetCaseId}, CorrelationId: {Cid}",
            msg.SourceCaseId, msg.TargetCaseId, msg.CorrelationId);

        try
        {
            Console.WriteLine($"Processing WFCaseLink {msg.SourceCaseId}");
        }
        catch (Exception ex)
        {
            var dto = msg.ToDto();
            // ثبت incident و افزایش retry
            await _service.AddIncidentAsync(dto, "PROCESS_FAILED", ex.Message);
            await _service.IncrementRetryAsync(dto, 10); // retry بعد از 10 دقیقه
        }
    }
}
