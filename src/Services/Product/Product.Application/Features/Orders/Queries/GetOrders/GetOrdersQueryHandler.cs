using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.Orders.Queries.GetOrders;

/// <summary>
/// Handler for retrieving all orders.
/// </summary>
public sealed class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<List<OrderResponseDto>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetAllOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<List<OrderResponseDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return Result<List<OrderResponseDto>>.Success(orders.ToDto());
    }
}

/// <summary>
/// Handler for retrieving an order by ID.
/// </summary>
public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderResponseDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            return Result<OrderResponseDto>.Failure(new Error(
                "Order.NotFound",
                $"Order with ID {request.Id} was not found."
            ));
        }

        return Result<OrderResponseDto>.Success(order.ToDto());
    }
}

/// <summary>
/// Handler for retrieving orders by status.
/// </summary>
public sealed class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, Result<List<OrderResponseDto>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersByStatusQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<List<OrderResponseDto>>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var status))
        {
            return Result<List<OrderResponseDto>>.Failure(new Error(
                "Order.InvalidStatus",
                $"Invalid order status: {request.Status}."
            ));
        }

        var orders = await _orderRepository.GetByStatusAsync(status, cancellationToken);
        return Result<List<OrderResponseDto>>.Success(orders.ToDto());
    }
}

/// <summary>
/// Handler for retrieving an order by order number.
/// </summary>
public sealed class GetOrderByOrderNumberQueryHandler : IRequestHandler<GetOrderByOrderNumberQuery, Result<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByOrderNumberQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderResponseDto>> Handle(GetOrderByOrderNumberQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(request.OrderNumber, cancellationToken);

        if (order is null)
        {
            return Result<OrderResponseDto>.Failure(new Error(
                "Order.NotFound",
                $"Order with number {request.OrderNumber} was not found."
            ));
        }

        return Result<OrderResponseDto>.Success(order.ToDto());
    }
}
