using Application.Interfaces;
using Application.Messaging.Contract;
using Domain.Entities;
using MassTransit;

namespace Application.Services;

public class WFCaseLinkService: IWFCaseLinkService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public WFCaseLinkService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task SendTestMessage()
    {
        await _publishEndpoint.Publish(new WFCaseLinkCreated
        {
            CreatedAt = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid().ToString()
        });
    }

    public async Task PublishLinkCreated(WFCaseLink entity)
    {
        await _publishEndpoint.Publish(new WFCaseLinkCreated
        {
            //CaseId = entity.Id,
            //SourceCaseId = entity.SourceCaseId,
            //SourceMainEntityId = entity.SourceMainEntityId,
            //TargetCaseId = entity.TargetCaseId,
            //TargetMainEntityId = entity.TargetMainEntityId,
            //CreatedAt = entity.CreatedAt,
            //Status = entity.Status.ToString(),
            //CorrelationId = entity.CorrelationId ?? Guid.NewGuid().ToString()
        });
    }
}
