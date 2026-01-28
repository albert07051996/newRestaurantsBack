using MediatR;
using Microsoft.AspNetCore.Http;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Commands.UpdateDish;

/// <summary>
/// კერძის განახლების Command
/// </summary>
public record UpdateDishCommand : IRequest<Result<DishResponseDto>>
{
    public Guid Id { get; init; }
    public string NameKa { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string DescriptionKa { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public decimal? Price { get; init; }
    public Guid DishCategoryId { get; init; }
    public int? PreparationTimeMinutes { get; init; }
    public int? Calories { get; init; }
    public int? SpicyLevel { get; init; }
    public string Ingredients { get; init; } = string.Empty;
    public string IngredientsEn { get; init; } = string.Empty;
    public string Volume { get; init; } = string.Empty;
    public string AlcoholContent { get; init; } = string.Empty;
    public bool IsVeganDish { get; init; }
    public bool IsAvailable { get; init; } = true;
    public string Comment { get; init; } = string.Empty;

    /// <summary>
    /// ახალი სურათის ფაილი (optional)
    /// </summary>
    public IFormFile? ImageFile { get; init; }

    /// <summary>
    /// ვიდეოს URL
    /// </summary>
    public string? VideoUrl { get; init; }
}