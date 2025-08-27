using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs;

/// <summary>
/// Data Transfer Object (DTO) representing a workflow case link.
/// This is a lightweight projection of <see cref="WFCaseLink"/> entity,
/// optimized for API communication between FrontOffice and BackOffice services.
/// It contains both the workflow link metadata and the business entity payload.
/// </summary>
public class WFCaseLinkDto
{
    /// <summary>
    /// Identifier of the source case in the workflow.
    /// Links always originate from this case.
    /// Part of the envelope for workflow correlation.
    /// </summary>
    public long SourceCaseId { get; set; }

    /// <summary>
    /// Type of the link (e.g., FOBO, dependency, relation).
    /// Serialized as string for JSON readability.
    /// Part of the envelope describing the relationship type.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WFCaseLinkType LinkType { get; set; }

    /// <summary>
    /// Date and time when the link was created (UTC).
    /// Envelope metadata for tracking the link lifecycle.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created this link (user id from the system).
    /// Envelope metadata, not part of business entity.
    /// </summary>
    public long CreatedByUserId { get; set; }

    /// <summary>
    /// Identifier of the main business entity associated with the source case.
    /// This can optionally be included in the payload, but kept in envelope for quick reference.
    /// </summary>
    public long SourceMainEntityId { get; set; }

    /// <summary>
    /// Current status of the link in the workflow lifecycle
    /// (e.g., Active, Closed, Suspended).
    /// Serialized as string for JSON readability.
    /// Envelope metadata for workflow state management.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WFCaseLinkStatus Status { get; set; }

    /// <summary>
    /// Identifier of the application where the source case belongs.
    /// Part of the envelope to know the origin system.
    /// </summary>
    public Guid SourceAppId { get; set; }

    /// <summary>
    /// Human-readable name of the source main entity.
    /// Useful for UI display, logging, or quick reference.
    /// </summary>
    public string SourceMainEntityName { get; set; } = "";

    /// <summary>
    /// Name of the workflow class for the source case.
    /// Provides context about the type of workflow being linked.
    /// Part of envelope metadata.
    /// </summary>
    public string SourceWFClassName { get; set; } = "";

    /// <summary>
    /// Identifier of the target case created or linked in the destination workflow.
    /// Optional; may be null if target case is not yet created.
    /// Part of envelope for correlation.
    /// </summary>
    public long? TargetCaseId { get; set; }

    /// <summary>
    /// Identifier of the main business entity in the target system.
    /// Optional; may be null if not yet available.
    /// Part of envelope metadata.
    /// </summary>
    public long? TargetMainEntityId { get; set; }

    /// <summary>
    /// Identifier of the application where the target case belongs.
    /// Part of envelope for routing and correlation.
    /// </summary>
    public Guid TargetAppId { get; set; }

    /// <summary>
    /// Human-readable name of the target main entity.
    /// Optional, for UI/logging purposes.
    /// </summary>
    public string TargetMainEntityName { get; set; } = "";

    /// <summary>
    /// Name of the workflow class for the target case.
    /// Part of envelope metadata for routing and context.
    /// </summary>
    public string TargetWFClassName { get; set; } = "";
    /// <summary>
    /// Optional serialized binary metadata for the workflow process.
    /// Can include SLA, priorities, business rules, or any other process-related info.
    /// </summary>

    public byte[]? ProcessMetaDataJson { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Payload containing the business entity data as JSON.
    /// This is the schema-less contract transferred between FrontOffice and BackOffice.
    /// Should contain fields like businessKeys, attributes, status, priority, attachments, etc.
    /// </summary>
    public string EntityJson { get; set; } = "{}";
    /// <summary>
    /// current task id which is saved in link 
    /// </summary>
    public long? currentTaskId { get; set; }
}

