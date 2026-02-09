namespace Product.Application.DTOs;

/// <summary>
/// DTO for order responses.
/// </summary>
public class OrderResponseDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? TableNumber { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid? TableSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}
