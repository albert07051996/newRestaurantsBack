using Microsoft.EntityFrameworkCore;
using Product.Application.Common.Interfaces;
using Product.Domain.Entities;

namespace Product.Persistence.Repositories;

public sealed class ReservationRepository : IReservationRepository
{
    private readonly ProductDbContext _context;

    public ReservationRepository(ProductDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Include(r => r.ReservationItems)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Reservation?> GetByReservationNumberAsync(string reservationNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Include(r => r.ReservationItems)
            .FirstOrDefaultAsync(r => r.ReservationNumber == reservationNumber, cancellationToken);
    }

    public async Task<List<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Include(r => r.ReservationItems)
            .OrderByDescending(r => r.ReservationDate)
            .ThenByDescending(r => r.ReservationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Reservation>> GetByStatusAsync(ReservationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Include(r => r.ReservationItems)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.ReservationDate)
            .ThenByDescending(r => r.ReservationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Reservation>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Include(r => r.ReservationItems)
            .Where(r => r.ReservationDate.Date == date.Date)
            .OrderBy(r => r.ReservationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Reservation>> GetUpcomingAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Reservations
            .Include(r => r.ReservationItems)
            .Where(r => r.ReservationDate >= today && r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Completed)
            .OrderBy(r => r.ReservationDate)
            .ThenBy(r => r.ReservationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
    }

    public void Update(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
    }
}
