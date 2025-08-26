using System.Text.Json;
using Domain.Entities;

namespace FrontOfficeAPI.Extensions;

public static class EntityJsonCreator
{
    public static string EntityJson(
        string linkId,
        string sourceAppId,
        long sourceCaseId,
        string sourceEntityType,
        string sourceEntityId,
        string targetAppId,
        string targetEntityType,
        Dictionary<string, string>? businessKeys = null,
        Dictionary<string, object>? attributes = null,
        string status = "Submitted",
        string priority = "Normal",
        DateTime? slaDueDate = null,
        List<Attachment>? attachments = null)
    {
        var jsonObj = new
        {
            schemaVersion = 1,
            linkId = linkId,
            source = new
            {
                appId = sourceAppId,
                caseId = sourceCaseId,
                entityType = sourceEntityType,
                entityId = sourceEntityId
            },
            target = new
            {
                appId = targetAppId,
                entityType = targetEntityType
            },
            businessKeys = businessKeys ?? new Dictionary<string, string>(),
            attributes = attributes ?? new Dictionary<string, object>(),
            status,
            priority,
            sla = slaDueDate.HasValue ? new { dueDate = slaDueDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") } : null,
            attachments = attachments ?? new List<Attachment>()
        };

        return JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true });
    }


}
