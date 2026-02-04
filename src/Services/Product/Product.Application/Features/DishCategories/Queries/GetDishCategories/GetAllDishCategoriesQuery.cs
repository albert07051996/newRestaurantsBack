using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.DishCategories.Queries.GetDishCategories;

/// <summary>
/// ყველა კატეგორიის მიღების Query
/// </summary>
public record GetAllDishCategoriesQuery : IRequest<Result<List<DishCategoryResponseDto>>>;

/// <summary>
/// კატეგორიის მიღება ID-ით
/// </summary>
public record GetDishCategoryByIdQuery(Guid Id) : IRequest<Result<DishCategoryResponseDto>>;
