using MediatR;
using Microsoft.AspNetCore.Http;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Commands.AddDish;

/// <summary>
/// ახალი კერძის დამატების Command
/// </summary>
public record AddDishCommand : IRequest<Result<DishResponseDto>>
{
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
    public string Comment { get; init; } = string.Empty;
    
    /// <summary>
    /// სურათის ფაილი (multipart/form-data)
    /// </summary>
    public IFormFile? ImageFile { get; init; }
    
    /// <summary>
    /// ვიდეოს URL (თუ არსებობს)
    /// </summary>
    public string? VideoUrl { get; init; }
}
