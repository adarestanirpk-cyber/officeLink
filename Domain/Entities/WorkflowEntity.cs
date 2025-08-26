namespace Domain.Entities;

public class WorkflowEntity
{
    public int Id { get; set; }

    // کل محتوای موجودیت به صورت JSON
    public string EntityJson { get; set; } = "{}";

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
