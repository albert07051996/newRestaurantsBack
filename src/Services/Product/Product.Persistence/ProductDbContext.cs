using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;

namespace Product.Persistence;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Dish> Dishes { get; set; }
    public DbSet<DishCategory> DishCategories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<TableSession> TableSessions { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationItem> ReservationItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        ConvertDateTimesToUtc();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertDateTimesToUtc();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ConvertDateTimesToUtc()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                foreach (var property in entry.Properties)
                {
                    if (property.CurrentValue is DateTime dt && dt.Kind != DateTimeKind.Utc)
                    {
                        property.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                }
            }
        }
    }
}
