using Product.Domain.Entities;

namespace Product.Application.Common.Interfaces;

public interface IProductRepository
{
    // Dish Methods
    Task<Dish?> GetDishByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Dish>> GetAllDishesAsync(CancellationToken cancellationToken = default);
    Task<List<Dish>> GetDeletedDishesAsync(CancellationToken cancellationToken = default); // ✅ ახალი
    Task AddDishAsync(Dish dish, CancellationToken cancellationToken = default);
    Task UpdateDishAsync(Dish dish, CancellationToken cancellationToken = default);
    Task DeleteDishAsync(Guid id, CancellationToken cancellationToken = default); // Soft Delete
    Task RestoreDishAsync(Guid id, CancellationToken cancellationToken = default); // ✅ ახალი

    // DishCategory Methods
    Task<bool> DishCategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<DishCategory?> GetDishCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<DishCategory>> GetAllDishCategoriesAsync(CancellationToken cancellationToken = default);

    // Save Changes
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}