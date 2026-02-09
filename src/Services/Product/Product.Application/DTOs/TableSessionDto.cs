namespace Product.Application.DTOs;

public class TableSessionResponseDto
{
    public Guid Id { get; set; }
    public string SessionNumber { get; set; } = string.Empty;
    public int TableNumber { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderResponseDto> Orders { get; set; } = new();
}
