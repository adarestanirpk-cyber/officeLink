using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.BackOffice.Persistence;

public class BackOfficeEntityRepository: IEntityRepository
{
    private readonly BackOfficeDbContext _db;

    public BackOfficeEntityRepository(BackOfficeDbContext db)
    {
        _db = db;
    }

    public async Task<int> AddAsync(WorkflowEntity entity, CancellationToken ct)
    {
        entity.CreatedAt = DateTime.UtcNow;
        _db.WorkflowEntities.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<WorkflowEntity?> GetByIdAsync(int id, CancellationToken ct)
        => await _db.WorkflowEntities.FindAsync(new object?[] { id }, ct);

    public async Task UpdateAsync(WorkflowEntity entity, CancellationToken ct)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _db.WorkflowEntities.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _db.WorkflowEntities.FindAsync(new object?[] { id }, ct);
        if (entity != null)
        {
            _db.WorkflowEntities.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
