using Inventory.Application.DTOs;
using Inventory.Application.Exceptions;
using Inventory.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;

    public AuthController(LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _loginUseCase.ExecuteAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
