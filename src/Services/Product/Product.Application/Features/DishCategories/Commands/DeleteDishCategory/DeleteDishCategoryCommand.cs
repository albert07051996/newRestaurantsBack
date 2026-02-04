using MediatR;
using Product.Application.Common.Models;

namespace Product.Application.Features.DishCategories.Commands.DeleteDishCategory;

/// <summary>
/// კატეგორიის წაშლის Command (Soft Delete)
/// </summary>
public record DeleteDishCategoryCommand(Guid Id) : IRequest<Result<bool>>;
