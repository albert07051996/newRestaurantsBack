using Product.Domain.Common;

namespace Product.Domain.Entities;

/// <summary>
/// Represents a customer order (Aggregate Root).
/// </summary>
public class Order : BaseEntity
{
    public string OrderNumber { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string? CustomerAddress { get; private set; }
    public OrderType OrderType { get; private set; }
    public OrderStatus Status { get; private set; }
    public int? TableNumber { get; private set; }
    public string? Notes { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Guid? TableSessionId { get; private set; }

    // Navigation Properties - backing field for EF Core
    private List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public TableSession? TableSession { get; private set; }

    // Private constructor for EF Core
    private Order() : base() { }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    public static Order Create(
        string customerName,
        string customerPhone,
        OrderType orderType,
        string? customerAddress = null,
        int? tableNumber = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required", nameof(customerName));

        if (string.IsNullOrWhiteSpace(customerPhone))
            throw new ArgumentException("Customer phone is required", nameof(customerPhone));

        if (orderType == OrderType.Delivery && string.IsNullOrWhiteSpace(customerAddress))
            throw new ArgumentException("Address is required for delivery orders", nameof(customerAddress));

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            CustomerAddress = customerAddress,
            OrderType = orderType,
            Status = OrderStatus.Pending,
            TableNumber = tableNumber,
            Notes = notes,
            TotalAmount = 0
        };

        return order;
    }

    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    public void AddItem(Guid dishId, string dishNameKa, string dishNameEn, int quantity, decimal unitPrice, string? specialInstructions = null)
    {
        var existingItem = _orderItems.FirstOrDefault(i => i.DishId == dishId && i.SpecialInstructions == specialInstructions);

        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = OrderItem.Create(
                Id,
                dishId,
                dishNameKa,
                dishNameEn,
                quantity,
                unitPrice,
                specialInstructions
            );
            _orderItems.Add(orderItem);
        }

        RecalculateTotalAmount();
        UpdateTimestamp();
    }

    /// <summary>
    /// Removes an item from the order.
    /// </summary>
    public void RemoveItem(Guid orderItemId)
    {
        var item = _orderItems.FirstOrDefault(i => i.Id == orderItemId);
        if (item is not null)
        {
            _orderItems.Remove(item);
            RecalculateTotalAmount();
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Updates the order status.
    /// </summary>
    public void UpdateStatus(OrderStatus newStatus)
    {
        // Business rule: Cannot change status of cancelled order
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot update status of a cancelled order");

        // Business rule: Cannot revert to previous statuses (except cancellation)
        if (newStatus != OrderStatus.Cancelled && newStatus < Status)
            throw new InvalidOperationException($"Cannot revert order status from {Status} to {newStatus}");

        Status = newStatus;
        UpdateTimestamp();
    }

    /// <summary>
    /// Cancels the order.
    /// </summary>
    public void Cancel()
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            throw new InvalidOperationException($"Cannot cancel order with status {Status}. Only Pending or Confirmed orders can be cancelled.");

        Status = OrderStatus.Cancelled;
        UpdateTimestamp();
    }

    /// <summary>
    /// Confirms the order.
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        if (!_orderItems.Any())
            throw new InvalidOperationException("Cannot confirm an empty order");

        Status = OrderStatus.Confirmed;
        UpdateTimestamp();
    }

    /// <summary>
    /// Updates customer information.
    /// </summary>
    public void UpdateCustomerInfo(string customerName, string customerPhone, string? customerAddress)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required", nameof(customerName));

        if (string.IsNullOrWhiteSpace(customerPhone))
            throw new ArgumentException("Customer phone is required", nameof(customerPhone));

        if (OrderType == OrderType.Delivery && string.IsNullOrWhiteSpace(customerAddress))
            throw new ArgumentException("Address is required for delivery orders", nameof(customerAddress));

        CustomerName = customerName;
        CustomerPhone = customerPhone;
        CustomerAddress = customerAddress;
        UpdateTimestamp();
    }

    public void SetTableSession(Guid tableSessionId)
    {
        TableSessionId = tableSessionId;
        UpdateTimestamp();
    }

    private void RecalculateTotalAmount()
    {
        TotalAmount = _orderItems.Sum(item => item.TotalPrice);
    }

    private static string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var uniquePart = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return $"ORD-{timestamp}-{uniquePart}";
    }
}
