using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Queries.GetDishes;

/// <summary>
/// ყველა კერძის მიღების Query
/// </summary>
public record GetAllDishesQuery : IRequest<Result<List<DishResponseDto>>>;
