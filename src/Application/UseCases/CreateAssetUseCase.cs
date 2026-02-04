using FluentValidation;
using Inventory.Application.DTOs;
using Inventory.Application.Exceptions;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.Application.UseCases;

public class CreateAssetUseCase
{
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetCategoryRepository _categoryRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateAssetRequest> _validator;

    public CreateAssetUseCase(
        IAssetRepository assetRepository,
        IAssetCategoryRepository categoryRepository,
        IAuditLogService auditLogService,
        ICurrentUserService currentUserService,
        IValidator<CreateAssetRequest> validator)
    {
        _assetRepository = assetRepository;
        _categoryRepository = categoryRepository;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
        _validator = validator;
    }

    public async Task<AssetDto> ExecuteAsync(CreateAssetRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 1. Uniqueness Check
        var existingAsset = await _assetRepository.GetByAssetCodeAsync(request.AssetCode, cancellationToken);
        if (existingAsset != null)
        {
            throw new BusinessException($"Asset with code '{request.AssetCode}' already exists.");
        }

        // 2. Category Existence Check
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(AssetCategory), request.CategoryId);
        }

        var asset = new Asset(
            Guid.NewGuid(),
            request.AssetCode,
            request.Name,
            request.CategoryId,
            request.PurchaseDate,
            request.Location
        );

        await _assetRepository.AddAsync(asset, cancellationToken);

        await _auditLogService.LogAsync(
            "Create",
            nameof(Asset),
            asset.Id,
            _currentUserService.UserId,
            $"Created asset {asset.AssetCode} ({asset.Name})",
            cancellationToken
        );

        return new AssetDto(
            asset.Id,
            asset.AssetCode,
            asset.Name,
            asset.CategoryId,
            asset.Status,
            asset.PurchaseDate,
            asset.Location,
            asset.AssignedToUserId
        );
    }
}
