using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
