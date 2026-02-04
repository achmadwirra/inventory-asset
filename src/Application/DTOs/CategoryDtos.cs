namespace Inventory.Application.DTOs;

public record AssetCategoryDto(Guid Id, string Name);

public record CreateCategoryRequest(string Name);

public record UpdateCategoryRequest(string Name);
