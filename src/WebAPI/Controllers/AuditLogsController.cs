using Inventory.Infrastructure.AuditLogging;
using Inventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.WebAPI.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = "Admin,ITStaff")]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditLogsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<AuditLogDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? action = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(action))
        {
            query = query.Where(a => a.Action == action);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Action = a.Action,
                PerformedByUserId = a.PerformedByUserId,
                Timestamp = a.Timestamp,
                OldValue = a.OldValue,
                NewValue = a.NewValue
            })
            .ToListAsync(cancellationToken);

        return Ok(new PaginatedResult<AuditLogDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        });
    }

    [HttpGet("actions")]
    public async Task<ActionResult<List<string>>> GetDistinctActions(CancellationToken cancellationToken)
    {
        var actions = await _context.AuditLogs
            .Select(a => a.Action)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync(cancellationToken);

        return Ok(actions);
    }
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string EntityName { get; set; } = default!;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = default!;
    public Guid PerformedByUserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string OldValue { get; set; } = default!;
    public string NewValue { get; set; } = default!;
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
