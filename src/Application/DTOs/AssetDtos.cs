using Inventory.Domain.Enums;

namespace Inventory.Application.DTOs;

public record AssetDto(
    Guid Id,
    string AssetCode,
    string Name,
    Guid CategoryId,
    AssetStatus Status,
    DateOnly PurchaseDate,
    string Location,
    Guid? AssignedToUserId
);

public record CreateAssetRequest(
    string AssetCode,
    string Name,
    Guid CategoryId,
    DateOnly PurchaseDate,
    string Location
);

public record AssignAssetRequest(
    Guid AssetId,
    Guid UserId
);
