using Application.DTOs;
using Application.Extensions;
using Application.Interfaces;
using Application.Mappers;
using Application.Messaging.Contract;
using Domain.Entities;
using Domain.ValueObjects;
using MassTransit;

namespace Application.Services;

public class WFCaseLinkService: IWFCaseLinkService
{
    private readonly IBus _bus;
    private readonly IWFCaseRepository _repository;

    public WFCaseLinkService(IBus bus, IWFCaseRepository repository)
    {
        _bus = bus;
        _repository = repository;
    }

    public async Task SendToBackOffice(WFCaseLink entity)
    {
        var sendEndpoint = await _bus.GetSendEndpoint(
            new Uri("queue:backoffice-wfcaselink-queue")
        );

        await sendEndpoint.Send(new WFCaseLinkCreated
        {
            SourceCaseId = entity.SourceCaseId,
            TargetCaseId = entity.TargetCaseId ?? 0,
            CreatedAt = entity.CreatedAt,
            CorrelationId = Guid.NewGuid().ToString(),
            LinkType = entity.LinkType,
            CreatedByUserId = entity.CreatedByUserId,
            SourceMainEntityId = entity.SourceMainEntityId,
            Status = entity.Status,
            SourceAppId = entity.SourceAppId,
            SourceMainEntityName = entity.SourceMainEntityName,
            SourceWFClassName = entity.SourceWFClassName,
            TargetMainEntityId = entity.TargetMainEntityId,
            TargetAppId = entity.TargetAppId,
            TargetMainEntityName = entity.TargetMainEntityName,
            TargetWFClassName = entity.TargetWFClassName,
            ProcessMetaDataJson = entity.ProcessMetaDataJson
        });
    }

    public async Task SendToFrontOffice(WFCaseLink entity)
    {
        var sendEndpoint = await _bus.GetSendEndpoint(
            new Uri("queue:frontoffice-wfcaselink-queue")
        );

        await sendEndpoint.Send(new WFCaseLinkCreated
        {
            SourceCaseId = entity.SourceCaseId,
            TargetCaseId = entity.TargetCaseId ?? 0,
            CreatedAt = entity.CreatedAt,
            CorrelationId = Guid.NewGuid().ToString(),
            LinkType = entity.LinkType,
            CreatedByUserId = entity.CreatedByUserId,
            SourceMainEntityId = entity.SourceMainEntityId,
            Status = entity.Status,
            SourceAppId = entity.SourceAppId,
            SourceMainEntityName = entity.SourceMainEntityName,
            SourceWFClassName = entity.SourceWFClassName,
            TargetMainEntityId = entity.TargetMainEntityId,
            TargetAppId = entity.TargetAppId,
            TargetMainEntityName = entity.TargetMainEntityName,
            TargetWFClassName = entity.TargetWFClassName,
            ProcessMetaDataJson = entity.ProcessMetaDataJson
        });
    }

    //public async Task PublishLinkCreated(WFCaseLink entity)
    //{
    //    await _publishEndpoint.Publish(new WFCaseLinkCreated
    //    {
    //        SourceCaseId = entity.SourceCaseId,
    //        SourceMainEntityId = entity.SourceMainEntityId,
    //        TargetCaseId = entity.TargetCaseId,
    //        TargetMainEntityId = entity.TargetMainEntityId,
    //        CreatedAt = entity.CreatedAt,
    //        Status = entity.Status,
    //        LinkType = entity.LinkType,
    //        CreatedByUserId = entity.CreatedByUserId,
    //        SourceAppId = entity.SourceAppId,
    //        SourceMainEntityName = entity.SourceMainEntityName, 
    //        SourceWFClassName=entity.TargetWFClassName,
    //        TargetAppId=entity.TargetAppId,
    //        TargetMainEntityName=entity.TargetMainEntityName,
    //        TargetWFClassName=entity.TargetWFClassName,
    //        ProcessMetaDataJson=entity.ProcessMetaDataJson,
    //        CorrelationId = Guid.NewGuid().ToString()
    //    });
    //}

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
