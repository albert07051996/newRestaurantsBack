using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.Reservations.Queries.GetReservations;

public sealed class GetAllReservationsQueryHandler : IRequestHandler<GetAllReservationsQuery, Result<List<ReservationResponseDto>>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetAllReservationsQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<List<ReservationResponseDto>>> Handle(GetAllReservationsQuery request, CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetAllAsync(cancellationToken);
        return Result<List<ReservationResponseDto>>.Success(reservations.ToDto());
    }
}

public sealed class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, Result<ReservationResponseDto>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationByIdQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<ReservationResponseDto>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (reservation is null)
        {
            return Result<ReservationResponseDto>.Failure(new Error(
                "Reservation.NotFound",
                $"Reservation with ID {request.Id} was not found."
            ));
        }

        return Result<ReservationResponseDto>.Success(reservation.ToDto());
    }
}

public sealed class GetReservationsByStatusQueryHandler : IRequestHandler<GetReservationsByStatusQuery, Result<List<ReservationResponseDto>>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationsByStatusQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<List<ReservationResponseDto>>> Handle(GetReservationsByStatusQuery request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ReservationStatus>(request.Status, ignoreCase: true, out var status))
        {
            return Result<List<ReservationResponseDto>>.Failure(new Error(
                "Reservation.InvalidStatus",
                $"Invalid reservation status: {request.Status}."
            ));
        }

        var reservations = await _reservationRepository.GetByStatusAsync(status, cancellationToken);
        return Result<List<ReservationResponseDto>>.Success(reservations.ToDto());
    }
}

public sealed class GetReservationsByDateQueryHandler : IRequestHandler<GetReservationsByDateQuery, Result<List<ReservationResponseDto>>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationsByDateQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<List<ReservationResponseDto>>> Handle(GetReservationsByDateQuery request, CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetByDateAsync(request.Date, cancellationToken);
        return Result<List<ReservationResponseDto>>.Success(reservations.ToDto());
    }
}

public sealed class GetReservationByNumberQueryHandler : IRequestHandler<GetReservationByNumberQuery, Result<ReservationResponseDto>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationByNumberQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<ReservationResponseDto>> Handle(GetReservationByNumberQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByReservationNumberAsync(request.ReservationNumber, cancellationToken);

        if (reservation is null)
        {
            return Result<ReservationResponseDto>.Failure(new Error(
                "Reservation.NotFound",
                $"Reservation with number {request.ReservationNumber} was not found."
            ));
        }

        return Result<ReservationResponseDto>.Success(reservation.ToDto());
    }
}
