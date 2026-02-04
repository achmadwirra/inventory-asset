using Inventory.Application.Interfaces;
using Inventory.Infrastructure.Persistence;

namespace Inventory.Infrastructure.AuditLogging;

public class AuditLogService : IAuditLogService
{
    // In a real app, this might insert into a DB via DbContext or another store.
    // Since we are implementing AppDbContext, we can use it here if registered or injected.
    // However, circle dependency might happen if DbContext uses AuditLogService.
    // Usually Audit is separate or part of the same transaction.
    // Implement as direct DB insert or similar.
    
    // For simplicity, let's assume we use the same DbContext but we need to resolve it or use a factory to avoid cycles if DbContext calls this.
    // But IAuditLogService is called by UseCases. So UseCase -> IAuditLogService -> DbContext is fine.
    
    private readonly AppDbContext _dbContext;

    public AuditLogService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAsync(string action, string entityName, Guid entityId, Guid userId, string details, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            PerformedByUserId = userId,
            Timestamp = DateTime.UtcNow,
            OldValue = "{}", // Manual logging might not have diffs easily available unless passed.
            NewValue = $"{{ \"details\": \"{details}\" }}" 
        };

        _dbContext.Set<AuditLog>().Add(log);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
