using Domain.Entities;

namespace Application.Interfaces;

public interface IEntityRepository
{
    // ایجاد و ذخیره موجودیت جدید در DB
    Task<int> AddAsync(WorkflowEntity entity, CancellationToken ct);

    // واکشی بر اساس Id
    Task<WorkflowEntity?> GetByIdAsync(int id, CancellationToken ct);

    // آپدیت موجودیت (مثلاً تغییر EntityJson یا UpdatedAt)
    Task UpdateAsync(WorkflowEntity entity, CancellationToken ct);

    // حذف موجودیت
    Task DeleteAsync(int id, CancellationToken ct);
}
