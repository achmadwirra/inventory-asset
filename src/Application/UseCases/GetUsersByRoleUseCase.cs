using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Enums;

namespace Inventory.Application.UseCases;

public class GetUsersByRoleUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUsersByRoleUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> ExecuteAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByRoleAsync(roleName, cancellationToken);
        // Since we filtered by roleName, we can assume all these users have that role name.
        // In a more complex scenario we might join Role table to be sure, but this suffices for now.
        return users.Select(u => new UserDto(u.Id, u.Email, roleName));
    }
}
