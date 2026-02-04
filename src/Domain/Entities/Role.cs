namespace Inventory.Domain.Entities;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Role(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    // For EF Core
    private Role() { Name = default!; }
}
