using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Reservations.Commands.UpdateReservationStatus;

public record UpdateReservationStatusCommand : IRequest<Result<ReservationResponseDto>>
{
    public Guid ReservationId { get; init; }
    public string Status { get; init; } = string.Empty;
}
