using Application.Interfaces;
using Domain.Entities;
using Infrastructure.FrontOffice.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.FrontOffice.Services;

public class FrontOfficeOutboxService: IOutboxService
{
    private readonly FrontOfficeOutboxDbContext _db;

    public FrontOfficeOutboxService(FrontOfficeOutboxDbContext db)
    {
        _db = db;
    }

    public async Task AddMessageAsync(OutboxMessage message, CancellationToken ct = default)
    {
        await _db.OutboxMessages.AddAsync(message, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<OutboxMessage>> GetPendingMessagesAsync(CancellationToken ct = default)
    {
        return await _db.OutboxMessages
            .Where(m => !m.ProcessedAt.HasValue)
            .ToListAsync(ct);
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken ct = default)
    {
        var msg = await _db.OutboxMessages.FindAsync([messageId], ct);
        if (msg != null)
        {
            msg.ProcessedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken ct = default)
    {
        return await _db.OutboxMessages
            .Where(m => !m.ProcessedAt.HasValue)
            .ToListAsync(ct);
    }
}
