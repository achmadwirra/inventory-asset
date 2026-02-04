using Inventory.Application.DTOs;
using Inventory.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly CreateAssetUseCase _createAssetUseCase;
    private readonly GetAssetsUseCase _getAssetsUseCase;
    private readonly AssignAssetUseCase _assignAssetUseCase;
    private readonly ReturnAssetUseCase _returnAssetUseCase;

    public AssetsController(
        CreateAssetUseCase createAssetUseCase,
        GetAssetsUseCase getAssetsUseCase,
        AssignAssetUseCase assignAssetUseCase,
        ReturnAssetUseCase returnAssetUseCase)
    {
        _createAssetUseCase = createAssetUseCase;
        _getAssetsUseCase = getAssetsUseCase;
        _assignAssetUseCase = assignAssetUseCase;
        _returnAssetUseCase = returnAssetUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getAssetsUseCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ITStaff")]
    public async Task<IActionResult> Create([FromBody] CreateAssetRequest request, CancellationToken cancellationToken)
    {
        var asset = await _createAssetUseCase.ExecuteAsync(request, cancellationToken);
        return Created($"/api/assets/{asset.Id}", asset);
    }

    [HttpPost("{id:guid}/assign")]
    [Authorize(Roles = "Admin,ITStaff")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignAssetBodyRequest body, CancellationToken cancellationToken)
    {
        var request = new AssignAssetRequest(id, body.UserId);
        var asset = await _assignAssetUseCase.ExecuteAsync(request, cancellationToken);
        return Ok(asset);
    }

    [HttpPost("{id:guid}/return")]
    public async Task<IActionResult> Return(Guid id, CancellationToken cancellationToken)
    {
        await _returnAssetUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}

public record AssignAssetBodyRequest(Guid UserId);
