using Inventory.Application.DTOs;
using Inventory.Application.UseCases;
using Inventory.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,ITStaff")]
public class UsersController : ControllerBase
{
    private readonly GetUsersByRoleUseCase _getUsersByRoleUseCase;

    public UsersController(GetUsersByRoleUseCase getUsersByRoleUseCase)
    {
        _getUsersByRoleUseCase = getUsersByRoleUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetByRole([FromQuery] string role, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return BadRequest("Role is required");
        }

        var users = await _getUsersByRoleUseCase.ExecuteAsync(role, cancellationToken);
        return Ok(users);
    }
}
