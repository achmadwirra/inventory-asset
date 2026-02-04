using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly AppDbContext _context;

    public AssetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Assets.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Asset?> GetByAssetCodeAsync(string assetCode, CancellationToken cancellationToken = default)
    {
        return await _context.Assets.FirstOrDefaultAsync(a => a.AssetCode == assetCode, cancellationToken);
    }

    public async Task<IEnumerable<Asset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Assets.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        await _context.Assets.AddAsync(asset, cancellationToken);
        // SaveChanges is typically called here or in UnitOfWork. 
        // UseCases call AddAsync then usually expect it to be saved.
        // Given I prefer simple patterns, I'll call SaveChangesAsync here unless UoW is enforced.
        // User didn't specify UoW.
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
