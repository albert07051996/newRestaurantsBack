namespace Product.Application.DTOs;

/// <summary>
/// Dish Response DTO
/// </summary>
public class DishResponseDto
{
    public Guid Id { get; set; }
    public string NameKa { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string DescriptionKa { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public Guid DishCategoryId { get; set; }
    public int? PreparationTimeMinutes { get; set; }
    public int? Calories { get; set; }
    public int? SpicyLevel { get; set; }
    public string Ingredients { get; set; } = string.Empty;
    public string IngredientsEn { get; set; } = string.Empty;
    public string Volume { get; set; } = string.Empty;
    public string AlcoholContent { get; set; } = string.Empty;
    public bool IsVeganDish { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
