using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class WFCaseLink
{
    public Guid Id { get; set; }
    public long SourceCaseId { get; set; }
    public long? TargetCaseId { get; set; }
    public WFCaseLinkType LinkType { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedByUserId { get; set; }
    public long SourceMainEntityId { get; set; }
    public long? TargetMainEntityId { get; set; }
    public WFCaseLinkStatus Status { get; set; }
    public Guid SourceAppId { get; set; }
    public Guid TargetAppId { get; set; }

    public string SourceMainEntityName { get; set; } = "";
    public string TargetMainEntityName { get; set; } = "";
    public string SourceWFClassName { get; set; } = "";
    public string TargetWFClassName { get; set; } = "";

    // ذخیره باینری در دیتابیس
    public byte[]? ProcessMetaDataJson { get; set; } = Array.Empty<byte>();
    [NotMapped]
    public ProcessMetaData ProcessMetaData
    {
        get
        {
            if (ProcessMetaDataJson == null || ProcessMetaDataJson.Length == 0)
                return new ProcessMetaData();
            var jsonString = System.Text.Encoding.UTF8.GetString(ProcessMetaDataJson);
            return JsonSerializer.Deserialize<ProcessMetaData>(jsonString)!;
        }
        set
        {
            var jsonString = JsonSerializer.Serialize(value);
            ProcessMetaDataJson = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }
    }
}
