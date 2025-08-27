using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class WFCaseRepositoryBase<TContext> :IWFCaseRepository
    where TContext : DbContext
{
    protected readonly TContext _context;

    public WFCaseRepositoryBase(TContext context)
    {
        _context = context;
    }

    public async Task<WFCaseLink?> GetByTaskIdAsync(long? taskId, CancellationToken ct = default)
    {
        return await _context.Set<WFCaseLink>()
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.currentTaskId == taskId, ct);
    }

    // Get by Id
    public async Task<WFCaseLink?> GetByIdAsync(Guid id, CancellationToken ct = default) => 
        await _context.Set<WFCaseLink>().FirstOrDefaultAsync(x => x.Id == id,ct);

    // Add new WFCaseLink
    public async Task AddAsync(WFCaseLink wfCaseLink,CancellationToken ct=default)
    {
        await _context.Set<WFCaseLink>().AddAsync(wfCaseLink, ct);
        await _context.SaveChangesAsync(ct);
    }

    //update state as failed
    public async Task UpdateWFStateToFailed(WFCaseLink wFCaseLink, CancellationToken ct = default)
    {
        wFCaseLink.Status = Domain.Enums.WFCaseLinkStatus.Failed;
        _context.Set<WFCaseLink>().Update(wFCaseLink);
        await _context.SaveChangesAsync(ct);
    }

    // Optional: Get all links for a source case
    public async Task<List<WFCaseLink>> GetBySourceCaseIdAsync(long sourceCaseId,CancellationToken ct=default) =>
        await _context.Set<WFCaseLink>().Where(x => x.SourceCaseId == sourceCaseId).ToListAsync(ct);

    // Update an existing WFCaseLink
    public async Task UpdateAsync(WFCaseLink wfCaseLink, CancellationToken ct = default)
    {
        _context.Set<WFCaseLink>().Update(wfCaseLink);
        await _context.SaveChangesAsync(ct);
    }

    // Optional: Delete a link
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity != null)
        {
            _context.Set<WFCaseLink>().Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }

    /// <summary>
    /// آیتم‌هایی که وضعیت Failed دارن و هنوز RetryDeadline شون نگذشته و RetryCount > 0 هست رو برمی‌گردونه
    /// </summary>
    public async Task<IEnumerable<WFCaseLink>> GetFailedLinksAsync(CancellationToken ct)
    {
        return await _context.Set<WFCaseLink>()
            .Where(x => x.Status == Domain.Enums.WFCaseLinkStatus.Failed)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task UpdateRetryStateAsync(WFCaseLink item, bool success, CancellationToken ct)
    {
        var entity = await _context.Set<WFCaseLink>().FirstOrDefaultAsync(x => x.Id == item.Id, ct);
        if (entity == null) return;

        var meta = entity.ProcessMetaData;

        if (success)
        {
            // ✅ موفقیت در ارسال
            entity.Status = Domain.Enums.WFCaseLinkStatus.Completed; // یا Completed بسته به سناریو
            meta.Retry.RetryCount = 0;
            meta.IncidentDetails.Clear();
        }
        else
        {
            // ❌ شکست در ارسال
            entity.Status = Domain.Enums.WFCaseLinkStatus.Failed;

            // اگر هنوز retry باقی مونده
            if (meta.Retry.RetryCount > 0)
                meta.Retry.RetryCount--;

            // لاگ خطا داخل IncidentDetails
            meta.IncidentDetails.Add(new IncidentDetails
            {
                Message = $"Retry failed at {DateTime.UtcNow:O}",
                Code = "RETRY_FAILED"
            });

            // زمان‌بندی retry بعدی
            meta.Retry.ResponseDeadline = DateTime.UtcNow.AddMinutes(meta.Retry.RetryAfterMinutes);
        }

        // serialize به بایت
        entity.ProcessMetaData = meta;

        _context.Set<WFCaseLink>().Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// دریافت WFCaseLink بر اساس TargetCaseId
    /// </summary>
    public async Task<WFCaseLink?> GetByCaseIdAsync(long targetCaseId, CancellationToken ct = default)
    {
        return await _context.Set<WFCaseLink>()
            .AsNoTracking() // برای جلوگیری از Tracking غیرضروری
            .FirstOrDefaultAsync(x => x.TargetCaseId == targetCaseId, ct);
    }

}
