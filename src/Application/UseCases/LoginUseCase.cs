using System.Security.Cryptography;
using System.Text;
using Inventory.Application.DTOs;
using Inventory.Application.Exceptions;
using Inventory.Application.Interfaces;

namespace Inventory.Application.UseCases;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUseCase(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            throw new BusinessException("Invalid email or password");
        }

        var passwordHash = HashPassword(request.Password);
        if (user.PasswordHash != passwordHash)
        {
            throw new BusinessException("Invalid email or password");
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        var roleName = role?.Name ?? "Employee";

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, roleName);

        return new LoginResponse(token, new UserDto(user.Id, user.Email, roleName));
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
