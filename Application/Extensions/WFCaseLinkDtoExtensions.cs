using Application.DTOs;
using Domain.Entities;
using System.Text.Json;

namespace Application.Extensions;

public static class WFCaseLinkDtoExtensions
{
    public static ProcessMetaData GetProcessMetaData(this WFCaseLinkDto dto)
    {
        if (dto.ProcessMetaDataJson == null || dto.ProcessMetaDataJson.Length == 0)
            return new ProcessMetaData();

        return JsonSerializer.Deserialize<ProcessMetaData>(dto.ProcessMetaDataJson)!;
    }

    public static void SetProcessMetaData(this WFCaseLinkDto dto, ProcessMetaData metaData)
    {
        dto.ProcessMetaDataJson = JsonSerializer.SerializeToUtf8Bytes(metaData);
    }
}
