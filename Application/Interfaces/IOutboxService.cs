using Domain.Entities;

namespace Application.Interfaces;

public interface IOutboxService
{
    Task AddMessageAsync(OutboxMessage message, CancellationToken ct = default);
    Task<List<OutboxMessage>> GetPendingMessagesAsync(CancellationToken ct = default);
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken ct = default);
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken ct = default);

}
