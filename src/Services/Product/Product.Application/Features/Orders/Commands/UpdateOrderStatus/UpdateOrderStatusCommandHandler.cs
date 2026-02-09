using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Handler for updating order status.
/// </summary>
public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderResponseDto>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate status
        if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var newStatus))
        {
            return Result<OrderResponseDto>.Failure(new Error(
                "Order.InvalidStatus",
                $"Invalid order status: {request.Status}. Valid statuses are: Pending, Confirmed, Preparing, Ready, Delivered, Cancelled."
            ));
        }

        // 2. Get order
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<OrderResponseDto>.Failure(new Error(
                "Order.NotFound",
                $"Order with ID {request.OrderId} was not found."
            ));
        }

        // 3. Update status
        try
        {
            order.UpdateStatus(newStatus);
        }
        catch (InvalidOperationException ex)
        {
            return Result<OrderResponseDto>.Failure(new Error("Order.StatusUpdateFailed", ex.Message));
        }

        // 4. Persist changes
        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderResponseDto>.Success(order.ToDto());
    }
}
