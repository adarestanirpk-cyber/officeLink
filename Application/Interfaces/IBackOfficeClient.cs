using Application.DTOs;

namespace Application.Interfaces;

public interface IBackOfficeClient
{
    Task<WFCaseLinkDto> SendLinkAsync(WFCaseLinkDto dto,  CancellationToken ct);
}
