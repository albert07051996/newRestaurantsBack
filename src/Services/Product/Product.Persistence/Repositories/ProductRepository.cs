using Microsoft.EntityFrameworkCore;
using Product.Application.Common.Interfaces;
using Product.Domain.Entities;

namespace Product.Persistence.Repositories;

/// <summary>
/// Repository implementation for Product domain operations.
/// Implements IDishRepository, IDishCategoryRepository and IUnitOfWork through IProductRepository.
/// </summary>
public sealed class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #region IDishRepository Implementation

    /// <inheritdoc />
    public async Task<Dish?> GetDishByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Dishes
            .Include(d => d.DishCategory)
            .Where(d => !d.IsDeleted)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Dish>> GetAllDishesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Dishes
            .Include(d => d.DishCategory)
            .Where(d => !d.IsDeleted)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Dish>> GetDeletedDishesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Dishes
            .Include(d => d.DishCategory)
            .Where(d => d.IsDeleted)
            .OrderByDescending(d => d.DeletedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddDishAsync(Dish dish, CancellationToken cancellationToken = default)
    {
        await _context.Dishes.AddAsync(dish, cancellationToken);
    }

    /// <inheritdoc />
    public void UpdateDish(Dish dish)
    {
        _context.Dishes.Update(dish);
    }

    /// <inheritdoc />
    public async Task DeleteDishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dish = await _context.Dishes
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

        if (dish is not null)
        {
            dish.SoftDelete();
            _context.Dishes.Update(dish);
        }
    }

    /// <inheritdoc />
    public async Task RestoreDishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dish = await _context.Dishes
            .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted, cancellationToken);

        if (dish is not null)
        {
            dish.Restore();
            _context.Dishes.Update(dish);
        }
    }

    #endregion

    #region IDishCategoryRepository Implementation

    /// <inheritdoc />
    public async Task<bool> DishCategoryExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DishCategories
            .AnyAsync(dc => dc.Id == id && !dc.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DishCategory?> GetDishCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DishCategories
            .Include(dc => dc.Dishes.Where(d => !d.IsDeleted))
            .Where(dc => !dc.IsDeleted)
            .FirstOrDefaultAsync(dc => dc.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<DishCategory>> GetAllDishCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DishCategories
            .Include(dc => dc.Dishes.Where(d => !d.IsDeleted))
            .Where(dc => !dc.IsDeleted)
            .OrderBy(dc => dc.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddDishCategoryAsync(DishCategory category, CancellationToken cancellationToken = default)
    {
        await _context.DishCategories.AddAsync(category, cancellationToken);
    }

    /// <inheritdoc />
    public void UpdateDishCategory(DishCategory category)
    {
        _context.DishCategories.Update(category);
    }

    /// <inheritdoc />
    public async Task DeleteDishCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.DishCategories
            .FirstOrDefaultAsync(dc => dc.Id == id && !dc.IsDeleted, cancellationToken);

        if (category is not null)
        {
            category.SoftDelete();
            _context.DishCategories.Update(category);
        }
    }

    #endregion

    #region IUnitOfWork Implementation

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
