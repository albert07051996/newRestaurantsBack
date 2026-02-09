using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Reservations.Queries.GetReservations;

public record GetAllReservationsQuery : IRequest<Result<List<ReservationResponseDto>>>;

public record GetReservationByIdQuery(Guid Id) : IRequest<Result<ReservationResponseDto>>;

public record GetReservationsByStatusQuery(string Status) : IRequest<Result<List<ReservationResponseDto>>>;

public record GetReservationsByDateQuery(DateTime Date) : IRequest<Result<List<ReservationResponseDto>>>;

public record GetReservationByNumberQuery(string ReservationNumber) : IRequest<Result<ReservationResponseDto>>;
