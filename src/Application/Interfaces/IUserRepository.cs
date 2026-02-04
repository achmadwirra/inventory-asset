using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default);
}
