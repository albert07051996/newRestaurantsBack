using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Features.Dishes.Commands.DeleteDish;
using Product.Application.Features.Dishes.Commands.UpdateDish;
using Product.Application.Features.Dishes.Queries.GetDishes;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("getAllMenuItems")]
    public async Task<IActionResult> GetAllMenuItems(CancellationToken cancellationToken)
    {
        var query = new GetAllDishesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateMenuItem(
        Guid id,
        [FromBody] UpdateDishCommand command,
        CancellationToken cancellationToken)
    {
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

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteMenuItem(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteDishCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
