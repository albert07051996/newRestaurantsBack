using Product.Domain.Common;

namespace Product.Domain.Entities;

public class TableSession : BaseEntity
{
    public string SessionNumber { get; private set; } = string.Empty;
    public int TableNumber { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public TableSessionStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }

    private List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private TableSession() : base() { }

    public static TableSession Create(
        int tableNumber,
        string customerName,
        string customerPhone)
    {
        if (tableNumber < 1)
            throw new ArgumentException("Table number must be at least 1", nameof(tableNumber));

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required", nameof(customerName));

        if (string.IsNullOrWhiteSpace(customerPhone))
            throw new ArgumentException("Customer phone is required", nameof(customerPhone));

        return new TableSession
        {
            SessionNumber = GenerateSessionNumber(),
            TableNumber = tableNumber,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            Status = TableSessionStatus.Active,
            TotalAmount = 0
        };
    }

    public void AddOrder(Order order)
    {
        if (Status == TableSessionStatus.Closed)
            throw new InvalidOperationException("Cannot add orders to a closed session");

        _orders.Add(order);
        RecalculateTotalAmount();
        UpdateTimestamp();
    }

    public void Close()
    {
        if (Status == TableSessionStatus.Closed)
            throw new InvalidOperationException("Session is already closed");

        Status = TableSessionStatus.Closed;
        UpdateTimestamp();
    }

    public void RecalculateTotalAmount()
    {
        TotalAmount = _orders.Sum(o => o.TotalAmount);
        UpdateTimestamp();
    }

    private static string GenerateSessionNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var uniquePart = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return $"SES-{timestamp}-{uniquePart}";
    }
}
