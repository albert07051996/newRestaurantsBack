using MediatR;
using Product.Application.Common.Models;

namespace Product.Application.Features.Reservations.Commands.CancelReservation;

public record CancelReservationCommand(Guid ReservationId) : IRequest<Result<bool>>;
