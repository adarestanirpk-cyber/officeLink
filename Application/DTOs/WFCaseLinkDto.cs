using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs;

/// <summary>
/// Data Transfer Object (DTO) representing a workflow case link.
/// This is a lightweight projection of <see cref="WFCaseLink"/> entity,
/// optimized for API communication between FrontOffice and BackOffice services.
/// </summary>
public class WFCaseLinkDto
{
    /// <summary>
    /// Identifier of the source case in the workflow.
    /// Links always originate from this case.
    /// </summary>
    public long SourceCaseId { get; set; }

    /// <summary>
    /// Type of the link (e.g., FOBO, dependency, relation).
    /// Serialized as string for JSON readability.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WFCaseLinkType LinkType { get; set; }

    /// <summary>
    /// Date and time when the link was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created this link (user id from the system).
    /// </summary>
    public long CreatedByUserId { get; set; }

    /// <summary>
    /// Identifier of the main business entity associated with the source case.
    /// </summary>
    public long SourceMainEntityId { get; set; }

    /// <summary>
    /// Current status of the link in the workflow lifecycle
    /// (e.g., Active, Closed, Suspended).
    /// Serialized as string for JSON readability.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WFCaseLinkStatus Status { get; set; }

    /// <summary>
    /// Identifier of the application where the source case belongs.
    /// </summary>
    public Guid SourceAppId { get; set; }

    /// <summary>
    /// Human-readable name of the source main entity.
    /// Useful for UI display or logging purposes.
    /// </summary>
    public string SourceMainEntityName { get; set; } = "";

    /// <summary>
    /// Name of the workflow class for the source case.
    /// Provides context about the type of workflow being linked.
    /// </summary>
    public string SourceWFClassName { get; set; } = "";

    // --- NEW: Target workflow case values ---
    public long? TargetCaseId { get; set; }
    public long? TargetMainEntityId { get; set; }
    public Guid TargetAppId { get; set; }
    public string TargetMainEntityName { get; set; } = "";
    public string TargetWFClassName { get; set; } = "";
    public byte[]? ProcessMetaDataJson { get; set; } = Array.Empty<byte>();
}

