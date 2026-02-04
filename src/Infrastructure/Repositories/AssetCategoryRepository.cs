using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class AssetCategoryRepository : IAssetCategoryRepository
{
    private readonly AppDbContext _context;

    public AssetCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AssetCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AssetCategories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<List<AssetCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AssetCategories
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<AssetCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.AssetCategories
            .FirstOrDefaultAsync(c => c.Name == name && !c.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(AssetCategory category, CancellationToken cancellationToken = default)
    {
        await _context.AssetCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AssetCategory category, CancellationToken cancellationToken = default)
    {
        _context.AssetCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasAssetsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets.AnyAsync(a => a.CategoryId == categoryId, cancellationToken);
    }
}
