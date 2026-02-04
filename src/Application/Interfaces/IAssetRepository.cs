using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Asset?> GetByAssetCodeAsync(string assetCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);
    Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default);
    // Task DeleteAsync(Guid id); // Not strictly requested but common. Not adding unless needed.
}
