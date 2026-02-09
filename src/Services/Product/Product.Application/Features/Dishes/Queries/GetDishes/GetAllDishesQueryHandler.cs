using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Queries.GetDishes;

/// <summary>
/// Handler for retrieving all active dishes.
/// </summary>
public sealed class GetAllDishesQueryHandler : IRequestHandler<GetAllDishesQuery, Result<List<DishResponseDto>>>
{
    private readonly IDishRepository _dishRepository;

    public GetAllDishesQueryHandler(IDishRepository dishRepository)
    {
        _dishRepository = dishRepository;
    }

    public async Task<Result<List<DishResponseDto>>> Handle(GetAllDishesQuery request, CancellationToken cancellationToken)
    {
        var dishes = await _dishRepository.GetAllDishesAsync(cancellationToken);
        return Result<List<DishResponseDto>>.Success(dishes.ToDto());
    }
}

