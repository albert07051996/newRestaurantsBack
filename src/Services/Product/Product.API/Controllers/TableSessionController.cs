using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Features.TableSessions.Commands.CloseTableSession;
using Product.Application.Features.TableSessions.Queries;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TableSessionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TableSessionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTableSessionByIdQuery(id), cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("active")]
    [Authorize]
    public async Task<IActionResult> GetActiveSessions(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetActiveTableSessionsQuery(), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllTableSessionsQuery(), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("table/{tableNumber:int}/active")]
    public async Task<IActionResult> GetActiveForTable(int tableNumber, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetActiveSessionForTableQuery(tableNumber), cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/close")]
    [Authorize]
    public async Task<IActionResult> CloseSession(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CloseTableSessionCommand(id), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }
}
