using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Queries.GetDishes;

/// <summary>
/// Handler for retrieving a dish by its identifier.
/// </summary>
public sealed class GetDishByIdQueryHandler : IRequestHandler<GetDishByIdQuery, Result<DishResponseDto>>
{
    private readonly IDishRepository _dishRepository;

    public GetDishByIdQueryHandler(IDishRepository dishRepository)
    {
        _dishRepository = dishRepository;
    }

    public async Task<Result<DishResponseDto>> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        var dish = await _dishRepository.GetDishByIdAsync(request.Id, cancellationToken);

        if (dish is null)
        {
            return Result<DishResponseDto>.Failure(new Error(
                "Dish.NotFound",
                $"Dish with ID {request.Id} was not found."
            ));
        }

        return Result<DishResponseDto>.Success(dish.ToDto());
    }
}
