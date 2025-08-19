using Application.Interfaces;
using Application.Messaging.Contract;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Messaging.Consumers;

public class WFCaseLinkConsumer : IConsumer<WFCaseLinkCreated>
{
    private readonly ILogger<WFCaseLinkConsumer> _logger;
    private readonly IWFCaseRepository _repository;
    public WFCaseLinkConsumer(IWFCaseRepository repository,ILogger<WFCaseLinkConsumer> logger)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<WFCaseLinkCreated> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Received message: {SourceCaseId} -> {TargetCaseId}, CorrelationId: {Cid}",
            msg.SourceCaseId, msg.TargetCaseId, msg.CorrelationId);

        //var entity = new WFCaseLink
        //{
        //    Id = message.CaseId,
        //    TargetCaseId = message.TargetCaseId,
        //    TargetMainEntityId = message.TargetMainEntityId,
        //    Status = Enum.Parse<WFCaseLinkStatus>(message.Status),
        //    CreatedAt = message.CreatedAt
        //};

        //await _repository.AddAsync(entity);

    }
}
