using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Orders.Queries.GetOrders;

/// <summary>
/// Query for retrieving all orders.
/// </summary>
public record GetAllOrdersQuery : IRequest<Result<List<OrderResponseDto>>>;

/// <summary>
/// Query for retrieving an order by ID.
/// </summary>
public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderResponseDto>>;

/// <summary>
/// Query for retrieving orders by status.
/// </summary>
public record GetOrdersByStatusQuery(string Status) : IRequest<Result<List<OrderResponseDto>>>;

/// <summary>
/// Query for retrieving an order by order number.
/// </summary>
public record GetOrderByOrderNumberQuery(string OrderNumber) : IRequest<Result<OrderResponseDto>>;
