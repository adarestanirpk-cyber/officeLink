using Application.DTOs;
using Application.Interfaces;
using Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FrontOffice.HttpClients;

public class BackOfficeClient : BaseClient, IBackOfficeClient
{
    protected override string Origin => "FrontOffice";

    public BackOfficeClient(HttpClient httpClient, ILogger<BackOfficeClient> logger, ICorrelationIdProvider correlationIdProvider)
        : base(httpClient, logger, correlationIdProvider) { }

    public Task<WFCaseLinkDto> SendLinkAsync(WFCaseLinkDto dto, CancellationToken ct)
        => SendLinkInternalAsync(dto, ct);
}
