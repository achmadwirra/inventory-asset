namespace Inventory.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
}
