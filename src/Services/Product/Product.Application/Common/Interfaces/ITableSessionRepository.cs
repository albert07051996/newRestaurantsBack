using Product.Domain.Entities;

namespace Product.Application.Common.Interfaces;

public interface ITableSessionRepository
{
    Task<TableSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TableSession?> GetActiveSessionForTableAsync(int tableNumber, CancellationToken cancellationToken = default);
    Task<List<TableSession>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<List<TableSession>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TableSession session, CancellationToken cancellationToken = default);
    void Update(TableSession session);
}
