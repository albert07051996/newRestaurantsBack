using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.Reservations.Commands.UpdateReservationStatus;

public sealed class UpdateReservationStatusCommandHandler : IRequestHandler<UpdateReservationStatusCommand, Result<ReservationResponseDto>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReservationStatusCommandHandler(IReservationRepository reservationRepository, IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReservationResponseDto>> Handle(UpdateReservationStatusCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ReservationStatus>(request.Status, ignoreCase: true, out var newStatus))
        {
            return Result<ReservationResponseDto>.Failure(new Error(
                "Reservation.InvalidStatus",
                $"Invalid reservation status: {request.Status}. Valid statuses are: Pending, Confirmed, Completed, Cancelled."
            ));
        }

        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
        {
            return Result<ReservationResponseDto>.Failure(new Error(
                "Reservation.NotFound",
                $"Reservation with ID {request.ReservationId} was not found."
            ));
        }

        try
        {
            reservation.UpdateStatus(newStatus);
        }
        catch (InvalidOperationException ex)
        {
            return Result<ReservationResponseDto>.Failure(new Error("Reservation.StatusUpdateFailed", ex.Message));
        }

        _reservationRepository.Update(reservation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ReservationResponseDto>.Success(reservation.ToDto());
    }
}
