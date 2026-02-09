namespace Product.Domain.Entities;

/// <summary>
/// Represents the status of an order.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order has been placed but not yet confirmed.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Order has been confirmed by the restaurant.
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Order is being prepared in the kitchen.
    /// </summary>
    Preparing = 3,

    /// <summary>
    /// Order is ready for pickup or delivery.
    /// </summary>
    Ready = 4,

    /// <summary>
    /// Order has been delivered to the customer.
    /// </summary>
    Delivered = 5,

    /// <summary>
    /// Order has been cancelled.
    /// </summary>
    Cancelled = 6
}
