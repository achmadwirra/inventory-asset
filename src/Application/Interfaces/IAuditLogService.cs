namespace Inventory.Application.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(string action, string entityName, Guid entityId, Guid userId, string details, CancellationToken cancellationToken = default);
}
