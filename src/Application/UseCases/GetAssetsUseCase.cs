using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.Application.UseCases;

public class GetAssetsUseCase
{
    private readonly IAssetRepository _assetRepository;

    public GetAssetsUseCase(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<IEnumerable<AssetDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var assets = await _assetRepository.GetAllAsync(cancellationToken);
        
        return assets.Select(a => new AssetDto(
            a.Id,
            a.AssetCode,
            a.Name,
            a.CategoryId,
            a.Status,
            a.PurchaseDate,
            a.Location,
            a.AssignedToUserId
        ));
    }
}
