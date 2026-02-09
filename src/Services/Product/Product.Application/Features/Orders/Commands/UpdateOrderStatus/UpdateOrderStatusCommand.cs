using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Command for updating order status.
/// </summary>
public record UpdateOrderStatusCommand : IRequest<Result<OrderResponseDto>>
{
    public Guid OrderId { get; init; }
    public string Status { get; init; } = string.Empty; // "Confirmed", "Preparing", "Ready", "Delivered"
}
