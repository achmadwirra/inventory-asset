using Inventory.Application.DTOs;
using Inventory.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.WebAPI.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize(Roles = "Admin,ITStaff")]
public class CategoriesController : ControllerBase
{
    private readonly GetCategoriesUseCase _getCategoriesUseCase;
    private readonly CreateCategoryUseCase _createCategoryUseCase;
    private readonly UpdateCategoryUseCase _updateCategoryUseCase;
    private readonly DeleteCategoryUseCase _deleteCategoryUseCase;

    public CategoriesController(
        GetCategoriesUseCase getCategoriesUseCase,
        CreateCategoryUseCase createCategoryUseCase,
        UpdateCategoryUseCase updateCategoryUseCase,
        DeleteCategoryUseCase deleteCategoryUseCase)
    {
        _getCategoriesUseCase = getCategoriesUseCase;
        _createCategoryUseCase = createCategoryUseCase;
        _updateCategoryUseCase = updateCategoryUseCase;
        _deleteCategoryUseCase = deleteCategoryUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<List<AssetCategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _getCategoriesUseCase.ExecuteAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<AssetCategoryDto>> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _createCategoryUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AssetCategoryDto>> Update(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _updateCategoryUseCase.ExecuteAsync(id, request, cancellationToken);
        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _deleteCategoryUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
