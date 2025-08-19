using Domain.Entities;

namespace Application.Interfaces;

public interface IRetryHandler
{
    Task<IEnumerable<WFCaseLink>> GetFailedItemsAsync(CancellationToken ct);
    Task<bool> ProcessItemAsync(WFCaseLink item, CancellationToken ct);
    Task MarkAsRetriedAsync(WFCaseLink item, bool success, CancellationToken ct);
}
