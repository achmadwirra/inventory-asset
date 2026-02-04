using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IAssetCategoryRepository
{
    Task<AssetCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AssetCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AssetCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(AssetCategory category, CancellationToken cancellationToken = default);
    Task UpdateAsync(AssetCategory category, CancellationToken cancellationToken = default);
    Task<bool> HasAssetsAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
