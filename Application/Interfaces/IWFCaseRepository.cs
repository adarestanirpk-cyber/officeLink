using Domain.Entities;

namespace Application.Interfaces;

public interface IWFCaseRepository
{
    Task AddAsync(WFCaseLink wfCaseLink, CancellationToken ct = default);
    Task<IEnumerable<WFCaseLink>> GetFailedLinksAsync(CancellationToken ct);
    Task UpdateRetryStateAsync(WFCaseLink item, bool success, CancellationToken ct);
    Task<WFCaseLink?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateWFStateToFailed(WFCaseLink wFCaseLink, CancellationToken ct = default);
    Task UpdateAsync(WFCaseLink wfCaseLink, CancellationToken ct = default);
}
