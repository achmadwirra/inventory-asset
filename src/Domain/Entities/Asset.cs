using Inventory.Domain.DomainEvents;
using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

public class Asset
{
    public Guid Id { get; private set; }
    public string AssetCode { get; private set; } // Unique
    public string Name { get; private set; }
    public Guid CategoryId { get; private set; }
    public AssetStatus Status { get; private set; }
    public DateOnly PurchaseDate { get; private set; }
    public string Location { get; private set; }
    public Guid? AssignedToUserId { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public Asset(Guid id, string assetCode, string name, Guid categoryId, DateOnly purchaseDate, string location)
    {
        Id = id;
        AssetCode = assetCode;
        Name = name;
        CategoryId = categoryId;
        Status = AssetStatus.InStock;
        PurchaseDate = purchaseDate;
        Location = location;
    }

    public void AssignTo(Guid userId)
    {
        if (Status != AssetStatus.InStock)
        {
            var statusName = Status switch
            {
                AssetStatus.Assigned => "already assigned to another user",
                AssetStatus.Maintenance => "under maintenance",
                AssetStatus.Retired => "retired",
                _ => Status.ToString()
            };
            throw new InvalidOperationException($"Cannot assign asset '{AssetCode}': Asset is {statusName}.");
        }

        var previousStatus = Status;
        Status = AssetStatus.Assigned;
        AssignedToUserId = userId;

        _domainEvents.Add(new AssetAssignedEvent(Id, userId, DateOnly.FromDateTime(DateTime.UtcNow)));
        _domainEvents.Add(new AssetStatusChangedEvent(Id, previousStatus, AssetStatus.Assigned));
    }

    public void Return()
    {
        if (Status != AssetStatus.Assigned)
        {
            var statusName = Status switch
            {
                AssetStatus.InStock => "not currently assigned (status: In Stock)",
                AssetStatus.Maintenance => "under maintenance",
                AssetStatus.Retired => "retired",
                _ => Status.ToString()
            };
            throw new InvalidOperationException($"Cannot return asset '{AssetCode}': Asset is {statusName}.");
        }

        var previousStatus = Status;
        Status = AssetStatus.InStock;
        AssignedToUserId = null;

        _domainEvents.Add(new AssetReturnedEvent(Id, DateOnly.FromDateTime(DateTime.UtcNow)));
        _domainEvents.Add(new AssetStatusChangedEvent(Id, previousStatus, AssetStatus.InStock));
    }

    public void ChangeStatus(AssetStatus newStatus)
    {
        if (Status == newStatus) return;

        // Validation logic can be expanded here
        if (Status == AssetStatus.Assigned && newStatus == AssetStatus.Retired)
        {
             // Maybe allow retiring directly? Or force return first?
             // Requirement says: "Asset TIDAK BOLEH di-assign jika status Maintenance atau Retired"
             // It doesn't explicitly restrict status changes FROM Assigned.
             // But usually, you should return it first. Let's assume you must return it first for safety.
             throw new InvalidOperationException("Cannot change status from Assigned directly without returning.");
        }

        var oldStatus = Status;
        Status = newStatus;
        
        if (newStatus != AssetStatus.Assigned)
        {
            AssignedToUserId = null; // Clear assignment if moved to Maintenance or Retired logic
        }

        _domainEvents.Add(new AssetStatusChangedEvent(Id, oldStatus, newStatus));
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
