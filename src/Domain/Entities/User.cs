namespace Inventory.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Guid RoleId { get; private set; }

    public User(Guid id, string email, string passwordHash, Guid roleId)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
    }

    // For EF Core
    private User()
    {
        Email = default!;
        PasswordHash = default!;
    }
}
