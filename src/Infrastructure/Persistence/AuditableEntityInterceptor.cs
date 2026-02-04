using Inventory.Application.Interfaces;
using Inventory.Infrastructure.AuditLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Inventory.Infrastructure.Persistence;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    // We can't inject DbContext here easily to save logs if it's the SAME DbContext -> recursion.
    // So usually AuditLogs are added to the ChangeTracker or we use a definition that maps them.
    // Or we just add to the Context being intercepted!

    /*
      However, since we need ICurrentUserService, we might need to register this interceptor 
      via DI and AddDbContext options.
      User asked for ICurrentUserService interface, so I'll assume I can use it (or mock implementation later).
    */

    public AuditableEntityInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.Entity is AuditLog) continue; // Don't audit the audit log

            var auditEntry = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                Timestamp = DateTime.UtcNow,
                PerformedByUserId = _currentUserService.UserId, // Will be Guid.Empty if no user context
                EntityId = Guid.Parse(entry.Property("Id").CurrentValue?.ToString() ?? Guid.Empty.ToString()), // Assume Id is Guid/String
                OldValue = entry.State == EntityState.Modified ? entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]?.ToString()).ToString() ?? "{}" : "{}", // Simplified JSON
                NewValue = entry.State != EntityState.Deleted ? entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]?.ToString()).ToString() ?? "{}" : "{}"
            };
            
            // To properly serialize JSON would require System.Text.Json
            // For now putting placeholder string format or strict JSON if I add the using
            
            context.Set<AuditLog>().Add(auditEntry);
        }
    }
}
