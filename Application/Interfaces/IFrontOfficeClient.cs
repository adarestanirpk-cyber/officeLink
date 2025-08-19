using Application.DTOs;

namespace Application.Interfaces;

public interface IFrontOfficeClient
{
    Task<WFCaseLinkDto> SendLinkAsync(WFCaseLinkDto dto, CancellationToken ct);
}
