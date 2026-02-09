using Product.Domain.Entities;

namespace Product.Application.Common.Interfaces;

/// <summary>
/// Repository interface for DishCategory entity operations.
/// Follows Interface Segregation Principle (ISP).
/// </summary>
public interface IDishCategoryRepository
{
    /// <summary>
    /// Checks if a dish category exists by its identifier.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if category exists and is not deleted; otherwise false.</returns>
    Task<bool> DishCategoryExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a dish category by its identifier (excludes soft-deleted).
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category if found; otherwise null.</returns>
    Task<DishCategory?> GetDishCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active dish categories (excludes soft-deleted).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of active categories ordered by DisplayOrder.</returns>
    Task<List<DishCategory>> GetAllDishCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new dish category to the repository.
    /// </summary>
    /// <param name="category">The category entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddDishCategoryAsync(DishCategory category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing category as updated in the change tracker.
    /// </summary>
    /// <param name="category">The category entity to update.</param>
    void UpdateDishCategory(DishCategory category);

    /// <summary>
    /// Soft-deletes a dish category by its identifier.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteDishCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}
