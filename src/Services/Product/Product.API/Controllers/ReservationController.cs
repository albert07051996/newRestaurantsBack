using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Features.Reservations.Commands.CancelReservation;
using Product.Application.Features.Reservations.Commands.CreateReservation;
using Product.Application.Features.Reservations.Commands.UpdateReservationStatus;
using Product.Application.Features.Reservations.Queries.GetReservations;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation(
        [FromBody] CreateReservationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetReservationById),
            new { id = result.Value!.Id },
            result.Value
        );
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllReservations(CancellationToken cancellationToken)
    {
        var query = new GetAllReservationsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReservationById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetReservationByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("by-number/{number}")]
    public async Task<IActionResult> GetReservationByNumber(string number, CancellationToken cancellationToken)
    {
        var query = new GetReservationByNumberQuery(number);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("status/{status}")]
    [Authorize]
    public async Task<IActionResult> GetReservationsByStatus(string status, CancellationToken cancellationToken)
    {
        var query = new GetReservationsByStatusQuery(status);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("date/{date}")]
    [Authorize]
    public async Task<IActionResult> GetReservationsByDate(DateTime date, CancellationToken cancellationToken)
    {
        var query = new GetReservationsByDateQuery(date);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateReservationStatus(
        Guid id,
        [FromBody] UpdateReservationStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateReservationStatusCommand
        {
            ReservationId = id,
            Status = request.Status
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> CancelReservation(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelReservationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}

public class UpdateReservationStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
