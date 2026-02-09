using Product.Domain.Entities;

namespace Product.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Order entity operations.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Gets an order by its identifier.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order if found; otherwise null.</returns>
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an order by its order number.
    /// </summary>
    /// <param name="orderNumber">The order number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order if found; otherwise null.</returns>
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all orders.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all orders ordered by creation date descending.</returns>
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets orders by status.
    /// </summary>
    /// <param name="status">The order status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of orders with the specified status.</returns>
    Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets today's orders.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of orders created today.</returns>
    Task<List<Order>> GetTodaysOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new order to the repository.
    /// </summary>
    /// <param name="order">The order entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing order as updated in the change tracker.
    /// </summary>
    /// <param name="order">The order entity to update.</param>
    void Update(Order order);
}
