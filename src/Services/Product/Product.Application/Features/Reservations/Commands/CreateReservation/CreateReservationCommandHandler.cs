using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.Reservations.Commands.CreateReservation;

public sealed class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Result<ReservationResponseDto>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationCommandHandler(
        IReservationRepository reservationRepository,
        IDishRepository dishRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _dishRepository = dishRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReservationResponseDto>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // 1. Parse time
        if (!TimeSpan.TryParseExact(request.ReservationTime, @"hh\:mm", null, out var time))
        {
            return Result<ReservationResponseDto>.Failure(new Error(
                "Reservation.InvalidTime",
                $"Invalid time format: {request.ReservationTime}. Expected format: HH:mm."
            ));
        }

        // 2. Create reservation
        Reservation reservation;
        try
        {
            reservation = Reservation.Create(
                customerName: request.CustomerName,
                customerPhone: request.CustomerPhone,
                reservationDate: request.ReservationDate,
                reservationTime: time,
                guestCount: request.GuestCount,
                tableNumber: request.TableNumber,
                notes: request.Notes
            );
        }
        catch (ArgumentException ex)
        {
            return Result<ReservationResponseDto>.Failure(new Error("Reservation.ValidationFailed", ex.Message));
        }

        // 3. Add items (optional)
        if (request.Items is not null && request.Items.Count > 0)
        {
            foreach (var item in request.Items)
            {
                var dish = await _dishRepository.GetDishByIdAsync(item.DishId, cancellationToken);
                if (dish is null)
                {
                    return Result<ReservationResponseDto>.Failure(new Error(
                        "Dish.NotFound",
                        $"Dish with ID {item.DishId} was not found."
                    ));
                }

                if (!dish.IsAvailable)
                {
                    return Result<ReservationResponseDto>.Failure(new Error(
                        "Dish.NotAvailable",
                        $"Dish '{dish.NameEn}' is currently not available."
                    ));
                }

                reservation.AddItem(
                    dishId: dish.Id,
                    dishNameKa: dish.NameKa,
                    dishNameEn: dish.NameEn,
                    quantity: item.Quantity,
                    unitPrice: dish.Price ?? 0,
                    specialInstructions: item.SpecialInstructions
                );
            }
        }

        // 4. Persist
        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ReservationResponseDto>.Success(reservation.ToDto());
    }
}
