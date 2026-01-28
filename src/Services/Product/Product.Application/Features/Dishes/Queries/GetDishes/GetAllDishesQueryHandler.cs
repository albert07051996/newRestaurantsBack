using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Queries.GetDishes;

public class GetAllDishesQueryHandler : IRequestHandler<GetAllDishesQuery, Result<List<DishResponseDto>>>
{
    private readonly IProductRepository _productRepository;

    public GetAllDishesQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<List<DishResponseDto>>> Handle(GetAllDishesQuery request, CancellationToken cancellationToken)
    {
        var dishes = await _productRepository.GetAllDishesAsync(cancellationToken);

        var response = dishes.Select(d => new DishResponseDto
        {
            Id = d.Id,
            NameKa = d.NameKa,
            NameEn = d.NameEn,
            DescriptionKa = d.DescriptionKa,
            DescriptionEn = d.DescriptionEn,
            Price = d.Price,
            DishCategoryId = d.DishCategoryId,
            PreparationTimeMinutes = d.PreparationTimeMinutes,
            Calories = d.Calories,
            SpicyLevel = d.SpicyLevel,
            Ingredients = d.Ingredients,
            IngredientsEn = d.IngredientsEn,
            Volume = d.Volume,
            AlcoholContent = d.AlcoholContent,
            IsVeganDish = d.IsVeganDish,
            Comment = d.Comment,
            ImageUrl = d.ImageUrl,
            VideoUrl = d.VideoUrl,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        }).ToList();

        return Result<List<DishResponseDto>>.Success(response);
    }
}
