using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class AssetAssignmentRepository : IAssetAssignmentRepository
{
    private readonly AppDbContext _context;

    public AssetAssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AssetAssignment assignment, CancellationToken cancellationToken = default)
    {
        await _context.AssetAssignments.AddAsync(assignment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AssetAssignment?> GetActiveAssignmentByAssetIdAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetAssignments
            .Where(a => a.AssetId == assetId && a.ReturnedAt == null)
            .OrderByDescending(a => a.AssignedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(AssetAssignment assignment, CancellationToken cancellationToken = default)
    {
        _context.AssetAssignments.Update(assignment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
