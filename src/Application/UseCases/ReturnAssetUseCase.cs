using Inventory.Application.Exceptions;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.Application.UseCases;

public class ReturnAssetUseCase
{
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetAssignmentRepository _assignmentRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserService _currentUserService;

    public ReturnAssetUseCase(
        IAssetRepository assetRepository,
        IAssetAssignmentRepository assignmentRepository,
        IAuditLogService auditLogService,
        ICurrentUserService currentUserService)
    {
        _assetRepository = assetRepository;
        _assignmentRepository = assignmentRepository;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset == null)
        {
            throw new NotFoundException(nameof(Asset), assetId);
        }

        try
        {
            asset.Return();
        }
        catch (InvalidOperationException ex)
        {
            throw new BusinessException(ex.Message);
        }

        var activeAssignment = await _assignmentRepository.GetActiveAssignmentByAssetIdAsync(assetId, cancellationToken);
        if (activeAssignment != null)
        {
            activeAssignment.Return(DateOnly.FromDateTime(DateTime.UtcNow));
            await _assignmentRepository.UpdateAsync(activeAssignment, cancellationToken);
        }

        await _assetRepository.UpdateAsync(asset, cancellationToken);

        await _auditLogService.LogAsync(
            "Return",
            nameof(Asset),
            asset.Id,
            _currentUserService.UserId,
            $"Returned asset {asset.AssetCode} ({asset.Name}). Status changed from Assigned to InStock.",
            cancellationToken
        );
    }
}
