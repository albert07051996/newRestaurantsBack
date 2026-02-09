using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.DishCategories.Queries.GetDishCategories;

/// <summary>
/// Handler for retrieving all active dish categories.
/// </summary>
public sealed class GetAllDishCategoriesQueryHandler : IRequestHandler<GetAllDishCategoriesQuery, Result<List<DishCategoryResponseDto>>>
{
    private readonly IDishCategoryRepository _categoryRepository;

    public GetAllDishCategoriesQueryHandler(IDishCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<List<DishCategoryResponseDto>>> Handle(GetAllDishCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllDishCategoriesAsync(cancellationToken);
        return Result<List<DishCategoryResponseDto>>.Success(categories.ToDto());
    }
}

/// <summary>
/// Handler for retrieving a dish category by its identifier.
/// </summary>
public sealed class GetDishCategoryByIdQueryHandler : IRequestHandler<GetDishCategoryByIdQuery, Result<DishCategoryResponseDto>>
{
    private readonly IDishCategoryRepository _categoryRepository;

    public GetDishCategoryByIdQueryHandler(IDishCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<DishCategoryResponseDto>> Handle(GetDishCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetDishCategoryByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return Result<DishCategoryResponseDto>.Failure(new Error(
                "DishCategory.NotFound",
                $"Category with ID {request.Id} was not found."
            ));
        }

        return Result<DishCategoryResponseDto>.Success(category.ToDto());
    }
}
