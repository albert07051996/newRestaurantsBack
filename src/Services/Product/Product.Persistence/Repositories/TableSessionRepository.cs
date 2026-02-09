using Microsoft.EntityFrameworkCore;
using Product.Application.Common.Interfaces;
using Product.Domain.Entities;

namespace Product.Persistence.Repositories;

public sealed class TableSessionRepository : ITableSessionRepository
{
    private readonly ProductDbContext _context;

    public TableSessionRepository(ProductDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<TableSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TableSessions
            .Include(s => s.Orders)
                .ThenInclude(o => o.OrderItems)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<TableSession?> GetActiveSessionForTableAsync(int tableNumber, CancellationToken cancellationToken = default)
    {
        return await _context.TableSessions
            .Include(s => s.Orders)
                .ThenInclude(o => o.OrderItems)
            .FirstOrDefaultAsync(s => s.TableNumber == tableNumber && s.Status == TableSessionStatus.Active, cancellationToken);
    }

    public async Task<List<TableSession>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TableSessions
            .Include(s => s.Orders)
                .ThenInclude(o => o.OrderItems)
            .Where(s => s.Status == TableSessionStatus.Active)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TableSession>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TableSessions
            .Include(s => s.Orders)
                .ThenInclude(o => o.OrderItems)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TableSession session, CancellationToken cancellationToken = default)
    {
        await _context.TableSessions.AddAsync(session, cancellationToken);
    }

    public void Update(TableSession session)
    {
        _context.TableSessions.Update(session);
    }
}
