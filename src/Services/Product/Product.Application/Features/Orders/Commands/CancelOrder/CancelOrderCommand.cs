using MediatR;
using Product.Application.Common.Models;

namespace Product.Application.Features.Orders.Commands.CancelOrder;

/// <summary>
/// Command for cancelling an order.
/// </summary>
public record CancelOrderCommand(Guid OrderId) : IRequest<Result<bool>>;
