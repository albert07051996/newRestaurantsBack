using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Features.DishCategories.Commands.AddDishCategory;
using Product.Application.Features.DishCategories.Commands.UpdateDishCategory;
using Product.Application.Features.DishCategories.Commands.DeleteDishCategory;
using Product.Application.Features.DishCategories.Queries.GetDishCategories;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DishCategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public DishCategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// ყველა კატეგორიის მიღება
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllDishCategories(CancellationToken cancellationToken)
    {
        var query = new GetAllDishCategoriesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// კატეგორიის მიღება ID-ით
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDishCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDishCategoryByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// ახალი კატეგორიის დამატება სურათით
    /// </summary>
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddDishCategory(
        [FromForm] AddDishCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetDishCategoryById),
            new { id = result.Value!.Id },
            result.Value
        );
    }

    /// <summary>
    /// კატეგორიის განახლება (სურათით)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateDishCategory(
        Guid id,
        [FromForm] UpdateDishCategoryCommand command,
        CancellationToken cancellationToken)
    {
        // ID-ის შემოწმება
        if (id != command.Id)
        {
            return BadRequest(new { Error = "URL-ის ID და Body-ს ID არ ემთხვევა" });
        }

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// კატეგორიის წაშლა (Soft Delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteDishCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteDishCategoryCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return NoContent(); // 204 No Content
    }
}
