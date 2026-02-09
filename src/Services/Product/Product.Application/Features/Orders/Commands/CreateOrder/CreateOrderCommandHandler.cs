using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Handler for creating a new order.
/// </summary>
public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDishRepository _dishRepository;
    private readonly ITableSessionRepository _tableSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IDishRepository dishRepository,
        ITableSessionRepository tableSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
        _tableSessionRepository = tableSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderResponseDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate order type
        if (!Enum.TryParse<OrderType>(request.OrderType, ignoreCase: true, out var orderType))
        {
            return Result<OrderResponseDto>.Failure(new Error(
                "Order.InvalidType",
                $"Invalid order type: {request.OrderType}. Valid types are: DineIn, TakeAway, Delivery."
            ));
        }

        // 2. Validate items
        if (request.Items is null || request.Items.Count == 0)
        {
            return Result<OrderResponseDto>.Failure(new Error(
                "Order.NoItems",
                "Order must contain at least one item."
            ));
        }

        // 3. Create order
        Order order;
        try
        {
            order = Order.Create(
                customerName: request.CustomerName,
                customerPhone: request.CustomerPhone,
                orderType: orderType,
                customerAddress: request.CustomerAddress,
                tableNumber: request.TableNumber,
                notes: request.Notes
            );
        }
        catch (ArgumentException ex)
        {
            return Result<OrderResponseDto>.Failure(new Error("Order.ValidationFailed", ex.Message));
        }

        // 4. Add items to order
        foreach (var item in request.Items)
        {
            var dish = await _dishRepository.GetDishByIdAsync(item.DishId, cancellationToken);
            if (dish is null)
            {
                return Result<OrderResponseDto>.Failure(new Error(
                    "Dish.NotFound",
                    $"Dish with ID {item.DishId} was not found."
                ));
            }

            if (!dish.IsAvailable)
            {
                return Result<OrderResponseDto>.Failure(new Error(
                    "Dish.NotAvailable",
                    $"Dish '{dish.NameEn}' is currently not available."
                ));
            }

            order.AddItem(
                dishId: dish.Id,
                dishNameKa: dish.NameKa,
                dishNameEn: dish.NameEn,
                quantity: item.Quantity,
                unitPrice: dish.Price ?? 0,
                specialInstructions: item.SpecialInstructions
            );
        }

        // 5. Handle TableSession for DineIn orders
        TableSession? session = null;
        if (orderType == OrderType.DineIn && request.TableNumber.HasValue)
        {
            if (request.TableSessionId.HasValue)
            {
                session = await _tableSessionRepository.GetByIdAsync(request.TableSessionId.Value, cancellationToken);
                if (session is null)
                {
                    return Result<OrderResponseDto>.Failure(new Error(
                        "TableSession.NotFound",
                        $"Table session with ID {request.TableSessionId} was not found."
                    ));
                }
                if (session.Status == TableSessionStatus.Closed)
                {
                    return Result<OrderResponseDto>.Failure(new Error(
                        "TableSession.Closed",
                        "Cannot add orders to a closed session."
                    ));
                }
            }
            else
            {
                // Check for existing active session on this table
                session = await _tableSessionRepository.GetActiveSessionForTableAsync(request.TableNumber.Value, cancellationToken);

                if (session is null)
                {
                    // Create new session
                    session = TableSession.Create(
                        tableNumber: request.TableNumber.Value,
                        customerName: request.CustomerName,
                        customerPhone: request.CustomerPhone
                    );
                    await _tableSessionRepository.AddAsync(session, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            // Set FK on order (don't use session.AddOrder + Update which causes graph traversal issues)
            order.SetTableSession(session.Id);
        }

        // 6. Persist order
        await _orderRepository.AddAsync(order, cancellationToken);

        // 7. Recalculate session total if applicable
        // EF Core fix-up automatically adds the order to session's Orders collection after AddAsync
        if (session != null)
        {
            session.RecalculateTotalAmount();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 8. Return response
        return Result<OrderResponseDto>.Success(order.ToDto());
    }
}
