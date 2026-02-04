using Microsoft.EntityFrameworkCore;
using Product.Application.Common.Interfaces;
using Product.Domain.Entities;

namespace Product.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    // ========== Dish Methods ==========

    /// <summary>
    /// კერძის მიღება ID-ით (მხოლოდ არა-წაშლილები)
    /// </summary>
    public async Task<Dish?> GetDishByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Dishes
            .Include(d => d.DishCategory)
            .Where(d => !d.IsDeleted) // ✅ გამორიცხე წაშლილები
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    /// <summary>
    /// ყველა კერძის მიღება (მხოლოდ არა-წაშლილები)
    /// </summary>
    public async Task<List<Dish>> GetAllDishesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Dishes
            .Include(d => d.DishCategory)
            .Where(d => !d.IsDeleted) // ✅ გამორიცხე წაშლილები
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// წაშლილი კერძების მიღება (Admin-ისთვის)
    /// </summary>
    public async Task<List<Dish>> GetDeletedDishesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Dishes
            .Include(d => d.DishCategory)
            .Where(d => d.IsDeleted) // ✅ მხოლოდ წაშლილები
            .OrderByDescending(d => d.DeletedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// ახალი კერძის დამატება
    /// </summary>
    public async Task AddDishAsync(Dish dish, CancellationToken cancellationToken = default)
    {
        await _context.Dishes.AddAsync(dish, cancellationToken);
    }

    /// <summary>
    /// კერძის განახლება
    /// </summary>
    public Task UpdateDishAsync(Dish dish, CancellationToken cancellationToken = default)
    {
        _context.Dishes.Update(dish);
        return Task.CompletedTask;
    }

    /// <summary>
    /// კერძის წაშლა (Soft Delete) - ფიზიკურად არ იშლება!
    /// </summary>
    public async Task DeleteDishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dish = await _context.Dishes
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

        if (dish != null)
        {
            dish.SoftDelete(); // ✅ Domain method
            _context.Dishes.Update(dish);
        }
    }

    /// <summary>
    /// წაშლილი კერძის აღდგენა
    /// </summary>
    public async Task RestoreDishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dish = await _context.Dishes
            .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted, cancellationToken);

        if (dish != null)
        {
            dish.Restore(); // ✅ Domain method
            _context.Dishes.Update(dish);
        }
    }

    // ========== DishCategory Methods ==========

    /// <summary>
    /// კატეგორიის არსებობის შემოწმება (არა-წაშლილი)
    /// </summary>
    public async Task<bool> DishCategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.DishCategories
            .AnyAsync(dc => dc.Id == categoryId && !dc.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// კატეგორიის მიღება ID-ით (მხოლოდ არა-წაშლილი)
    /// </summary>
    public async Task<DishCategory?> GetDishCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DishCategories
            .Include(dc => dc.Dishes.Where(d => !d.IsDeleted)) // ✅ გამორიცხე წაშლილი კერძები
            .Where(dc => !dc.IsDeleted)
            .FirstOrDefaultAsync(dc => dc.Id == id, cancellationToken);
    }

    /// <summary>
    /// ყველა კატეგორიის მიღება (მხოლოდ არა-წაშლილები)
    /// </summary>
    public async Task<List<DishCategory>> GetAllDishCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DishCategories
            .Include(dc => dc.Dishes.Where(d => !d.IsDeleted)) // ✅ გამორიცხე წაშლილი კერძები
            .Where(dc => !dc.IsDeleted)
            .OrderBy(dc => dc.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// ახალი კატეგორიის დამატება
    /// </summary>
    public async Task AddDishCategoryAsync(DishCategory category, CancellationToken cancellationToken = default)
    {
        await _context.DishCategories.AddAsync(category, cancellationToken);
    }

    /// <summary>
    /// კატეგორიის განახლება
    /// </summary>
    public Task UpdateDishCategoryAsync(DishCategory category, CancellationToken cancellationToken = default)
    {
        _context.DishCategories.Update(category);
        return Task.CompletedTask;
    }

    /// <summary>
    /// კატეგორიის წაშლა (Soft Delete)
    /// </summary>
    public async Task DeleteDishCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.DishCategories
            .FirstOrDefaultAsync(dc => dc.Id == id && !dc.IsDeleted, cancellationToken);

        if (category != null)
        {
            category.SoftDelete(); // Domain method
            _context.DishCategories.Update(category);
        }
    }

    // ========== Save Changes ==========

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}