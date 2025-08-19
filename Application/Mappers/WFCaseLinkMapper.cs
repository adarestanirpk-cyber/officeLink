using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers;

public static class WFCaseLinkMapper
{
    public static WFCaseLinkDto ToDto(this WFCaseLink entity)
    {
        return new WFCaseLinkDto
        {
            SourceCaseId = entity.SourceCaseId,
            LinkType = entity.LinkType,
            CreatedAt = entity.CreatedAt,
            CreatedByUserId = entity.CreatedByUserId,
            SourceMainEntityId = entity.SourceMainEntityId,
            Status = entity.Status,
            SourceAppId = entity.SourceAppId,
            SourceMainEntityName = entity.SourceMainEntityName,
            SourceWFClassName = entity.SourceWFClassName,
        };
    }

    public static WFCaseLink ToEntity(this WFCaseLinkDto dto)
    {
        return new WFCaseLink
        {
            SourceCaseId = dto.SourceCaseId,
            LinkType = dto.LinkType,
            CreatedAt = dto.CreatedAt,
            CreatedByUserId = dto.CreatedByUserId,
            SourceMainEntityId = dto.SourceMainEntityId,
            Status = dto.Status,
            SourceAppId = dto.SourceAppId,
            SourceMainEntityName = dto.SourceMainEntityName,
            SourceWFClassName = dto.SourceWFClassName,


            TargetMainEntityId = dto.TargetMainEntityId,
            TargetAppId = dto.TargetAppId != Guid.Empty ? dto.TargetAppId : Guid.NewGuid(),
            TargetMainEntityName = dto.TargetMainEntityName ?? "",
            TargetWFClassName = dto.TargetWFClassName ?? "",
            ProcessMetaDataJson = dto.ProcessMetaDataJson ?? Array.Empty<byte>()
        };
    }
}
