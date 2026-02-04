namespace Inventory.Domain.Entities;

public class AssetAssignment
{
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public Guid UserId { get; private set; }
    public DateOnly AssignedAt { get; private set; }
    public DateOnly? ReturnedAt { get; private set; }

    public AssetAssignment(Guid id, Guid assetId, Guid userId, DateOnly assignedAt)
    {
        Id = id;
        AssetId = assetId;
        UserId = userId;
        AssignedAt = assignedAt;
    }

    public void Return(DateOnly returnedAt)
    {
        if (ReturnedAt.HasValue)
        {
            throw new InvalidOperationException("Asset is already returned.");
        }
        ReturnedAt = returnedAt;
    }
}
