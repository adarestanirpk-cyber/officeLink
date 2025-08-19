using Domain.Enums;

namespace Application.Messaging.Contract;

public record WFCaseLinkCreated
{
    public long SourceCaseId { get; init; }
    public int TargetCaseId { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CorrelationId { get; init; } = "";
    public WFCaseLinkType LinkType { get; set; }
    public long CreatedByUserId { get; set; }
    public long SourceMainEntityId { get; set; }
    public WFCaseLinkStatus Status { get; set; }
    public Guid SourceAppId { get; set; }
    public string SourceMainEntityName { get; set; } = "";
    public string SourceWFClassName { get; set; } = "";
    public long? TargetMainEntityId { get; set; }
    public Guid TargetAppId { get; set; }
    public string TargetMainEntityName { get; set; } = "";
    public string TargetWFClassName { get; set; } = "";
    public byte[]? ProcessMetaDataJson { get; set; } = Array.Empty<byte>();
}
