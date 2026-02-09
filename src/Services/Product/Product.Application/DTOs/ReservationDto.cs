namespace Product.Application.DTOs;

public class ReservationResponseDto
{
    public Guid Id { get; set; }
    public string ReservationNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public string ReservationTime { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public int TableNumber { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ReservationItemResponseDto> Items { get; set; } = new();
}

public class ReservationItemResponseDto
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

public class ReservationItemRequestDto
{
    public Guid DishId { get; set; }
    public int Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}
