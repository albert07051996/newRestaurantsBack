using Product.Domain.Common;

namespace Product.Domain.Entities;

/// <summary>
/// Represents a single item within an order.
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid DishId { get; private set; }
    public string DishNameKa { get; private set; } = string.Empty;
    public string DishNameEn { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string? SpecialInstructions { get; private set; }

    // Navigation Properties
    public Order? Order { get; private set; }

    // Private constructor for EF Core
    private OrderItem() : base() { }

    /// <summary>
    /// Creates a new order item.
    /// </summary>
    public static OrderItem Create(
        Guid orderId,
        Guid dishId,
        string dishNameKa,
        string dishNameEn,
        int quantity,
        decimal unitPrice,
        string? specialInstructions = null)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID is required", nameof(orderId));

        if (dishId == Guid.Empty)
            throw new ArgumentException("Dish ID is required", nameof(dishId));

        if (string.IsNullOrWhiteSpace(dishNameKa))
            throw new ArgumentException("Georgian dish name is required", nameof(dishNameKa));

        if (string.IsNullOrWhiteSpace(dishNameEn))
            throw new ArgumentException("English dish name is required", nameof(dishNameEn));

        if (quantity < 1)
            throw new ArgumentException("Quantity must be at least 1", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        return new OrderItem
        {
            OrderId = orderId,
            DishId = dishId,
            DishNameKa = dishNameKa,
            DishNameEn = dishNameEn,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = quantity * unitPrice,
            SpecialInstructions = specialInstructions
        };
    }

    /// <summary>
    /// Updates the quantity of this order item.
    /// </summary>
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity < 1)
            throw new ArgumentException("Quantity must be at least 1", nameof(newQuantity));

        Quantity = newQuantity;
        TotalPrice = Quantity * UnitPrice;
        UpdateTimestamp();
    }
}
