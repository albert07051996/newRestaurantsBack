using MediatR;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Queries.GetDishes;

/// <summary>
/// კერძის ID-ით მიღების Query
/// </summary>
public record GetDishByIdQuery(Guid Id) : IRequest<Result<DishResponseDto>>;
