using Product.Domain.Common;

namespace Product.Domain.Entities;

public class ReservationItem : BaseEntity
{
    public Guid ReservationId { get; private set; }
    public Guid DishId { get; private set; }
    public string DishNameKa { get; private set; } = string.Empty;
    public string DishNameEn { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string? SpecialInstructions { get; private set; }

    public Reservation? Reservation { get; private set; }

    private ReservationItem() : base() { }

    public static ReservationItem Create(
        Guid reservationId,
        Guid dishId,
        string dishNameKa,
        string dishNameEn,
        int quantity,
        decimal unitPrice,
        string? specialInstructions = null)
    {
        if (reservationId == Guid.Empty)
            throw new ArgumentException("Reservation ID is required", nameof(reservationId));

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

        return new ReservationItem
        {
            ReservationId = reservationId,
            DishId = dishId,
            DishNameKa = dishNameKa,
            DishNameEn = dishNameEn,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = quantity * unitPrice,
            SpecialInstructions = specialInstructions
        };
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity < 1)
            throw new ArgumentException("Quantity must be at least 1", nameof(newQuantity));

        Quantity = newQuantity;
        TotalPrice = Quantity * UnitPrice;
        UpdateTimestamp();
    }
}
