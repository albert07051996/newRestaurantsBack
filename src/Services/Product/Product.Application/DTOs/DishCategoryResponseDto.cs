namespace Product.Application.DTOs;

/// <summary>
/// DishCategory Response DTO
/// </summary>
public class DishCategoryResponseDto
{
    public Guid Id { get; set; }
    public string NameKa { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionKa { get; set; }
    public string? DescriptionEn { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
