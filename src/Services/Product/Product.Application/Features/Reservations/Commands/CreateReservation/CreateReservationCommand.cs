using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Reservations.Commands.CreateReservation;

public record CreateReservationCommand : IRequest<Result<ReservationResponseDto>>
{
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;
    public DateTime ReservationDate { get; init; }
    public string ReservationTime { get; init; } = string.Empty;
    public int GuestCount { get; init; }
    public int TableNumber { get; init; }
    public string? Notes { get; init; }
    public List<ReservationItemRequestDto> Items { get; init; } = new();
}
