namespace Product.Application.Common.Interfaces;

/// <summary>
/// Composite repository interface for Product domain operations.
/// Combines IDishRepository, IDishCategoryRepository and IUnitOfWork for convenience.
/// </summary>
/// <remarks>
/// For new code, prefer injecting the specific interfaces (IDishRepository, IDishCategoryRepository)
/// and IUnitOfWork separately following Interface Segregation Principle (ISP).
/// </remarks>
public interface IProductRepository : IDishRepository, IDishCategoryRepository, IUnitOfWork
{
}
