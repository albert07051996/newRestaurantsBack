using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.DishCategories.Queries.GetDishCategories;

public class GetAllDishCategoriesQueryHandler : IRequestHandler<GetAllDishCategoriesQuery, Result<List<DishCategoryResponseDto>>>
{
    private readonly IProductRepository _productRepository;

    public GetAllDishCategoriesQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<List<DishCategoryResponseDto>>> Handle(GetAllDishCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _productRepository.GetAllDishCategoriesAsync(cancellationToken);

        var response = categories.Select(MapToResponseDto).ToList();

        return Result<List<DishCategoryResponseDto>>.Success(response);
    }

    private static DishCategoryResponseDto MapToResponseDto(DishCategory category)
    {
        return new DishCategoryResponseDto
        {
            Id = category.Id,
            NameKa = category.NameKa,
            NameEn = category.NameEn,
            DescriptionKa = category.DescriptionKa,
            DescriptionEn = category.DescriptionEn,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}

public class GetDishCategoryByIdQueryHandler : IRequestHandler<GetDishCategoryByIdQuery, Result<DishCategoryResponseDto>>
{
    private readonly IProductRepository _productRepository;

    public GetDishCategoryByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<DishCategoryResponseDto>> Handle(GetDishCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _productRepository.GetDishCategoryByIdAsync(request.Id, cancellationToken);

        if (category == null)
        {
            return Result<DishCategoryResponseDto>.Failure(new Error(
                "DishCategory.NotFound",
                $"კატეგორია ID {request.Id} ვერ მოიძებნა."
            ));
        }

        var response = MapToResponseDto(category);
        return Result<DishCategoryResponseDto>.Success(response);
    }

    private static DishCategoryResponseDto MapToResponseDto(DishCategory category)
    {
        return new DishCategoryResponseDto
        {
            Id = category.Id,
            NameKa = category.NameKa,
            NameEn = category.NameEn,
            DescriptionKa = category.DescriptionKa,
            DescriptionEn = category.DescriptionEn,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}
