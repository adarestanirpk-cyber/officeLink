using Application.DTOs;

namespace Application.Interfaces;

public interface IClient
{
    Task<WFCaseLinkDto> SendLinkAsync(WFCaseLinkDto dto, CancellationToken ct);
}
