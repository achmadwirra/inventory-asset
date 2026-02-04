using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IAssetAssignmentRepository
{
    Task AddAsync(AssetAssignment assignment, CancellationToken cancellationToken = default);
    Task<AssetAssignment?> GetActiveAssignmentByAssetIdAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task UpdateAsync(AssetAssignment assignment, CancellationToken cancellationToken = default);
}
