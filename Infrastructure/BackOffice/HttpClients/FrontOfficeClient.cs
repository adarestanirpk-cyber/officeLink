using Application.DTOs;
using Application.Interfaces;
using Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackOffice.HttpClients;

public class FrontOfficeClient : BaseClient, IFrontOfficeClient
{
    protected override string Origin => "BackOffice";

    public FrontOfficeClient(HttpClient httpClient, ILogger<FrontOfficeClient> logger, ICorrelationIdProvider correlationIdProvider)
        : base(httpClient, logger, correlationIdProvider) { }

    public Task<WFCaseLinkDto> SendLinkAsync(WFCaseLinkDto dto, CancellationToken ct)
        => SendLinkInternalAsync(dto, ct);
}
