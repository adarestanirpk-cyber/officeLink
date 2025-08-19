using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class RetryHandler : IRetryHandler
{
    private readonly IWFCaseRepository _repository;
    private readonly IClient _backOfficeClient;
    private readonly ILogger<RetryHandler> _logger;

    public RetryHandler(IWFCaseRepository repository, IClient backOfficeClient, ILogger<RetryHandler> logger)
    {
        _repository = repository;
        _backOfficeClient = backOfficeClient;
        _logger = logger;
    }


    public Task<IEnumerable<WFCaseLink>> GetFailedItemsAsync(CancellationToken ct)
            => _repository.GetFailedLinksAsync(ct);

    public async Task<bool> ProcessItemAsync(WFCaseLink item, CancellationToken ct)
    {
        try
        {
            var dto = item.ToDto();
            await _backOfficeClient.SendLinkAsync(dto, ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Retry failed for WFCaseLink {Id}", item.Id);
            return false;
        }
    }

    public Task MarkAsRetriedAsync(WFCaseLink item, bool success, CancellationToken ct)
           => _repository.UpdateRetryStateAsync(item, success, ct);
}
