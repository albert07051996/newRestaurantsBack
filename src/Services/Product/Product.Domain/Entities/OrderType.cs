namespace Product.Domain.Entities;

/// <summary>
/// Represents the type of order.
/// </summary>
public enum OrderType
{
    /// <summary>
    /// Customer dines at the restaurant.
    /// </summary>
    DineIn = 1,

    /// <summary>
    /// Customer picks up the order.
    /// </summary>
    TakeAway = 2,

    /// <summary>
    /// Order is delivered to customer's address.
    /// </summary>
    Delivery = 3
}
