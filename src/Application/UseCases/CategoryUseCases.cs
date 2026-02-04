using FluentValidation;
using Inventory.Application.DTOs;
using Inventory.Application.Exceptions;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.Application.UseCases;

public class GetCategoriesUseCase
{
    private readonly IAssetCategoryRepository _categoryRepository;

    public GetCategoriesUseCase(IAssetCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<AssetCategoryDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(c => new AssetCategoryDto(c.Id, c.Name)).ToList();
    }
}

public class CreateCategoryUseCase
{
    private readonly IAssetCategoryRepository _categoryRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateCategoryRequest> _validator;

    public CreateCategoryUseCase(
        IAssetCategoryRepository categoryRepository,
        IAuditLogService auditLogService,
        ICurrentUserService currentUserService,
        IValidator<CreateCategoryRequest> validator)
    {
        _categoryRepository = categoryRepository;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
        _validator = validator;
    }

    public async Task<AssetCategoryDto> ExecuteAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Check for duplicate name
        var existingCategory = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingCategory != null)
        {
            throw new BusinessException($"Category with name '{request.Name}' already exists.");
        }

        var category = new AssetCategory(Guid.NewGuid(), request.Name);
        await _categoryRepository.AddAsync(category, cancellationToken);

        await _auditLogService.LogAsync(
            "Create",
            nameof(AssetCategory),
            category.Id,
            _currentUserService.UserId,
            $"Created category '{category.Name}'",
            cancellationToken
        );

        return new AssetCategoryDto(category.Id, category.Name);
    }
}

public class UpdateCategoryUseCase
{
    private readonly IAssetCategoryRepository _categoryRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<UpdateCategoryRequest> _validator;

    public UpdateCategoryUseCase(
        IAssetCategoryRepository categoryRepository,
        IAuditLogService auditLogService,
        ICurrentUserService currentUserService,
        IValidator<UpdateCategoryRequest> validator)
    {
        _categoryRepository = categoryRepository;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
        _validator = validator;
    }

    public async Task<AssetCategoryDto> ExecuteAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(AssetCategory), id);
        }

        // Check for duplicate name (excluding current)
        var existingCategory = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingCategory != null && existingCategory.Id != id)
        {
            throw new BusinessException($"Category with name '{request.Name}' already exists.");
        }

        var oldName = category.Name;
        category.UpdateName(request.Name);
        await _categoryRepository.UpdateAsync(category, cancellationToken);

        await _auditLogService.LogAsync(
            "Update",
            nameof(AssetCategory),
            category.Id,
            _currentUserService.UserId,
            $"Updated category name from '{oldName}' to '{category.Name}'",
            cancellationToken
        );

        return new AssetCategoryDto(category.Id, category.Name);
    }
}

public class DeleteCategoryUseCase
{
    private readonly IAssetCategoryRepository _categoryRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCategoryUseCase(
        IAssetCategoryRepository categoryRepository,
        IAuditLogService auditLogService,
        ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(AssetCategory), id);
        }

        // Check if category has assets
        var hasAssets = await _categoryRepository.HasAssetsAsync(id, cancellationToken);
        if (hasAssets)
        {
            throw new BusinessException($"Cannot delete category '{category.Name}': Category still has assets assigned to it. Please reassign or delete the assets first.");
        }

        category.SoftDelete();
        await _categoryRepository.UpdateAsync(category, cancellationToken);

        await _auditLogService.LogAsync(
            "Delete",
            nameof(AssetCategory),
            category.Id,
            _currentUserService.UserId,
            $"Deleted category '{category.Name}'",
            cancellationToken
        );
    }
}
