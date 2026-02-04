namespace Inventory.Domain.Entities;

public class AssetCategory
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public AssetCategory(Guid id, string name)
    {
        Id = id;
        Name = name;
        IsDeleted = false;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Category name cannot be empty.");
        }
        Name = name;
    }

    public void SoftDelete()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("Category is already deleted.");
        }
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}
