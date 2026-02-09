using Product.Domain.Entities;

namespace Product.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Dish entity operations.
/// Follows Interface Segregation Principle (ISP).
/// </summary>
public interface IDishRepository
{
    /// <summary>
    /// Gets a dish by its identifier (excludes soft-deleted).
    /// </summary>
    /// <param name="id">The dish identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The dish if found; otherwise null.</returns>
    Task<Dish?> GetDishByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active dishes (excludes soft-deleted).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of active dishes.</returns>
    Task<List<Dish>> GetAllDishesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all soft-deleted dishes (for admin restore functionality).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of deleted dishes.</returns>
    Task<List<Dish>> GetDeletedDishesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new dish to the repository.
    /// </summary>
    /// <param name="dish">The dish entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddDishAsync(Dish dish, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing dish as updated in the change tracker.
    /// </summary>
    /// <param name="dish">The dish entity to update.</param>
    void UpdateDish(Dish dish);

    /// <summary>
    /// Soft-deletes a dish by its identifier.
    /// </summary>
    /// <param name="id">The dish identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteDishAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a soft-deleted dish.
    /// </summary>
    /// <param name="id">The dish identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RestoreDishAsync(Guid id, CancellationToken cancellationToken = default);
}
