using Application.DTOs;
using Application.Interfaces;
using Application.Messaging.Contract;
using Domain.Entities;
using MassTransit;
using Application.Extensions;
using Application.Mappers;

namespace Application.Services;

public class WFCaseLinkService: IWFCaseLinkService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IWFCaseRepository _repository;

    public WFCaseLinkService(IPublishEndpoint publishEndpoint, IWFCaseRepository repository)
    {
        _publishEndpoint = publishEndpoint;
        _repository = repository;
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

    public async Task IncrementRetryAsync(WFCaseLinkDto dto, int retryAfterMinutes = 5)
    {
        var metadata = dto.GetProcessMetaData();
        metadata.Retry.RetryCount++;
        metadata.Retry.RetryAfterMinutes = retryAfterMinutes;
        metadata.Retry.ResponseDeadline = DateTime.UtcNow.AddMinutes(retryAfterMinutes);
        dto.SetProcessMetaData(metadata);

        await _repository.UpdateAsync(dto.ToEntity());
    }

    public async Task AddIncidentAsync(WFCaseLinkDto dto, string code, string message)
    {
        var metadata = dto.GetProcessMetaData();
        metadata.IncidentDetails.Add(new IncidentDetails { Code = code, Message = message });
        dto.SetProcessMetaData(metadata);

        await _repository.UpdateAsync(dto.ToEntity());
    }
}
