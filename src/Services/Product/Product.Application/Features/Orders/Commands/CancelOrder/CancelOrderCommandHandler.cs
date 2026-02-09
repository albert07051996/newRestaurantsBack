using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;

namespace Product.Application.Features.Orders.Commands.CancelOrder;

/// <summary>
/// Handler for cancelling an order.
/// </summary>
public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<bool>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Get order
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<bool>.Failure(new Error(
                "Order.NotFound",
                $"Order with ID {request.OrderId} was not found."
            ));
        }

        // 2. Cancel order
        try
        {
            order.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(new Error("Order.CancelFailed", ex.Message));
        }

        // 3. Persist changes
        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
