using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Queries.GetDishes;

public class GetDishByIdQueryHandler : IRequestHandler<GetDishByIdQuery, Result<DishResponseDto>>
{
    private readonly IProductRepository _productRepository;

    public GetDishByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<DishResponseDto>> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        var dish = await _productRepository.GetDishByIdAsync(request.Id, cancellationToken);

        if (dish == null)
        {
            return Result<DishResponseDto>.Failure(new Error(
                "Dish.NotFound",
                $"Dish with ID {request.Id} not found."
            ));
        }

        var response = new DishResponseDto
        {
            Id = dish.Id,
            NameKa = dish.NameKa,
            NameEn = dish.NameEn,
            DescriptionKa = dish.DescriptionKa,
            DescriptionEn = dish.DescriptionEn,
            Price = dish.Price,
            DishCategoryId = dish.DishCategoryId,
            PreparationTimeMinutes = dish.PreparationTimeMinutes,
            Calories = dish.Calories,
            SpicyLevel = dish.SpicyLevel,
            Ingredients = dish.Ingredients,
            IngredientsEn = dish.IngredientsEn,
            Volume = dish.Volume,
            AlcoholContent = dish.AlcoholContent,
            IsVeganDish = dish.IsVeganDish,
            Comment = dish.Comment,
            ImageUrl = dish.ImageUrl,
            VideoUrl = dish.VideoUrl,
            CreatedAt = dish.CreatedAt,
            UpdatedAt = dish.UpdatedAt
        };

        return Result<DishResponseDto>.Success(response);
    }
}
