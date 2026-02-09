using Product.Domain.Common;

namespace Product.Domain.Entities;

public class Reservation : BaseEntity
{
    public string ReservationNumber { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public DateTime ReservationDate { get; private set; }
    public TimeSpan ReservationTime { get; private set; }
    public int GuestCount { get; private set; }
    public int TableNumber { get; private set; }
    public string? Notes { get; private set; }
    public ReservationStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }

    private List<ReservationItem> _reservationItems = new();
    public IReadOnlyCollection<ReservationItem> ReservationItems => _reservationItems.AsReadOnly();

    private Reservation() : base() { }

    public static Reservation Create(
        string customerName,
        string customerPhone,
        DateTime reservationDate,
        TimeSpan reservationTime,
        int guestCount,
        int tableNumber,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required", nameof(customerName));

        if (string.IsNullOrWhiteSpace(customerPhone))
            throw new ArgumentException("Customer phone is required", nameof(customerPhone));

        if (guestCount < 1)
            throw new ArgumentException("Guest count must be at least 1", nameof(guestCount));

        if (tableNumber < 1)
            throw new ArgumentException("Table number must be at least 1", nameof(tableNumber));

        return new Reservation
        {
            ReservationNumber = GenerateReservationNumber(),
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            ReservationDate = reservationDate.Date,
            ReservationTime = reservationTime,
            GuestCount = guestCount,
            TableNumber = tableNumber,
            Notes = notes,
            Status = ReservationStatus.Pending,
            TotalAmount = 0
        };
    }

    public void AddItem(Guid dishId, string dishNameKa, string dishNameEn, int quantity, decimal unitPrice, string? specialInstructions = null)
    {
        var existingItem = _reservationItems.FirstOrDefault(i => i.DishId == dishId && i.SpecialInstructions == specialInstructions);

        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var item = ReservationItem.Create(
                Id,
                dishId,
                dishNameKa,
                dishNameEn,
                quantity,
                unitPrice,
                specialInstructions
            );
            _reservationItems.Add(item);
        }

        RecalculateTotalAmount();
        UpdateTimestamp();
    }

    public void UpdateStatus(ReservationStatus newStatus)
    {
        if (Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Cannot update status of a cancelled reservation");

        Status = newStatus;
        UpdateTimestamp();
    }

    public void Cancel()
    {
        if (Status == ReservationStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed reservation");

        if (Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Reservation is already cancelled");

        Status = ReservationStatus.Cancelled;
        UpdateTimestamp();
    }

    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
            throw new InvalidOperationException("Only pending reservations can be confirmed");

        Status = ReservationStatus.Confirmed;
        UpdateTimestamp();
    }

    public void RecalculateTotalAmount()
    {
        TotalAmount = _reservationItems.Sum(item => item.TotalPrice);
    }

    private static string GenerateReservationNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var uniquePart = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return $"RES-{timestamp}-{uniquePart}";
    }
}
