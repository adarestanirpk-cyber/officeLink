using Application.DTOs;
using Application.Messaging.Contract;

namespace Application.Extensions;

public static class WFCaseLinkCreatedExtensions
{
    public static WFCaseLinkDto ToDto(this WFCaseLinkCreated msg)
    {
        return new WFCaseLinkDto
        {
            SourceCaseId = msg.SourceCaseId,
            TargetCaseId = msg.TargetCaseId,
            ProcessMetaDataJson = msg.ProcessMetaDataJson
        };
    }
}
