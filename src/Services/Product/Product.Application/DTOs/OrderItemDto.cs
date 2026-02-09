namespace Product.Application.DTOs;

/// <summary>
/// DTO for order item in requests.
/// </summary>
public class OrderItemRequestDto
{
    public Guid DishId { get; set; }
    public int Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}

/// <summary>
/// DTO for order item in responses.
/// </summary>
public class OrderItemResponseDto
{
    public Guid Id { get; set; }
    public Guid DishId { get; set; }
    public string DishNameKa { get; set; } = string.Empty;
    public string DishNameEn { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? SpecialInstructions { get; set; }
}
