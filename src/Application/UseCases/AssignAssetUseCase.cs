using FluentValidation;
using Inventory.Application.DTOs;
using Inventory.Application.Exceptions;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.Application.UseCases;

public class AssignAssetUseCase
{
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetAssignmentRepository _assignmentRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<AssignAssetRequest> _validator;

    public AssignAssetUseCase(
        IAssetRepository assetRepository,
        IAssetAssignmentRepository assignmentRepository,
        IAuditLogService auditLogService,
        ICurrentUserService currentUserService,
        IValidator<AssignAssetRequest> validator)
    {
        _assetRepository = assetRepository;
        _assignmentRepository = assignmentRepository;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
        _validator = validator;
    }

    public async Task<AssetDto> ExecuteAsync(AssignAssetRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset == null)
        {
            throw new NotFoundException(nameof(Asset), request.AssetId);
        }

        try
        {
            asset.AssignTo(request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            throw new BusinessException(ex.Message);
        }

        var assignment = new AssetAssignment(
            Guid.NewGuid(),
            asset.Id,
            request.UserId,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        await _assignmentRepository.AddAsync(assignment, cancellationToken);
        await _assetRepository.UpdateAsync(asset, cancellationToken);

        await _auditLogService.LogAsync(
            "Assign",
            nameof(Asset),
            asset.Id,
            _currentUserService.UserId,
            $"Assigned asset {asset.AssetCode} ({asset.Name}) to user {request.UserId}. Status changed from InStock to Assigned.",
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
