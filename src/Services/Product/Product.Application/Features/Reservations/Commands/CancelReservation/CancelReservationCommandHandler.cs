using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;

namespace Product.Application.Features.Reservations.Commands.CancelReservation;

public sealed class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, Result<bool>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationCommandHandler(IReservationRepository reservationRepository, IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
        {
            return Result<bool>.Failure(new Error(
                "Reservation.NotFound",
                $"Reservation with ID {request.ReservationId} was not found."
            ));
        }

        try
        {
            reservation.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(new Error("Reservation.CancelFailed", ex.Message));
        }

        _reservationRepository.Update(reservation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
