namespace Inventory.Infrastructure.AuditLogging;

public class AuditLog
{
    public Guid Id { get; set; }
    public string EntityName { get; set; } = default!;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = default!;
    public Guid PerformedByUserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string OldValue { get; set; } = default!; // JSON
    public string NewValue { get; set; } = default!; // JSON
}
