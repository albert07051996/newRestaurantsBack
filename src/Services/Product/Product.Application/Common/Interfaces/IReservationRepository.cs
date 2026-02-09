using Product.Domain.Entities;

namespace Product.Application.Common.Interfaces;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Reservation?> GetByReservationNumberAsync(string reservationNumber, CancellationToken cancellationToken = default);
    Task<List<Reservation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Reservation>> GetByStatusAsync(ReservationStatus status, CancellationToken cancellationToken = default);
    Task<List<Reservation>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<List<Reservation>> GetUpcomingAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    void Update(Reservation reservation);
}
