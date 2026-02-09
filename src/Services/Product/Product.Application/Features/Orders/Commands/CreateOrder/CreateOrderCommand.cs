using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Command for creating a new order.
/// </summary>
public record CreateOrderCommand : IRequest<Result<OrderResponseDto>>
{
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;
    public string? CustomerAddress { get; init; }
    public string OrderType { get; init; } = string.Empty; // "DineIn", "TakeAway", "Delivery"
    public int? TableNumber { get; init; }
    public string? Notes { get; init; }
    public Guid? TableSessionId { get; init; }
    public List<OrderItemRequestDto> Items { get; init; } = new();
}
