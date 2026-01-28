using MediatR;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Features.Dishes.Commands.AddDish;
using Product.Application.Features.Dishes.Commands.UpdateDish;
using Product.Application.Features.Dishes.Commands.DeleteDish;
using Product.Application.Features.Dishes.Commands.RestoreDish;
using Product.Application.Features.Dishes.Queries.GetDishes;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DishController : ControllerBase
{
    private readonly IMediator _mediator;

    public DishController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// ყველა კერძის მიღება
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllDishes(CancellationToken cancellationToken)
    {
        var query = new GetAllDishesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// კერძის მიღება ID-ით
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDishById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDishByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// ახალი კერძის დამატება სურათით
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddDish(
        [FromForm] AddDishCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetDishById),
            new { id = result.Value.Id },
            result.Value
        );
    }

    /// <summary>
    /// კერძის განახლება (სურათით)
    /// </summary>
    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateDish(
        Guid id,
        [FromForm] UpdateDishCommand command,
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
    /// კერძის წაშლა (Soft Delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDish(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteDishCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return NoContent(); // 204 No Content
    }

    /// <summary>
    /// წაშლილი კერძის აღდგენა
    /// </summary>
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> RestoreDish(Guid id, CancellationToken cancellationToken)
    {
        var command = new RestoreDishCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(new { Message = "კერძი წარმატებით აღდგა" });
    }
}