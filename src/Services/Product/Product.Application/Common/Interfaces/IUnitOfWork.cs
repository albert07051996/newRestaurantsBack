namespace Product.Application.Common.Interfaces;

/// <summary>
/// Unit of Work pattern interface for managing transaction boundaries.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of affected rows.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
