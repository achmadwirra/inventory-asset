using Inventory.Domain.Enums;

namespace Inventory.Domain.DomainEvents;

public interface IDomainEvent { }

public record AssetAssignedEvent(Guid AssetId, Guid UserId, DateOnly AssignedAt) : IDomainEvent;

public record AssetReturnedEvent(Guid AssetId, DateOnly ReturnedAt) : IDomainEvent;

public record AssetStatusChangedEvent(Guid AssetId, AssetStatus OldStatus, AssetStatus NewStatus) : IDomainEvent;
