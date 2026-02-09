using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Entities;

namespace Product.Persistence.Configurations;

/// <summary>
/// EF Core configuration for OrderItem entity.
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.DishNameKa)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.DishNameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(i => i.TotalPrice)
            .HasPrecision(18, 2);

        builder.Property(i => i.SpecialInstructions)
            .HasMaxLength(500);

        // Index for order queries
        builder.HasIndex(i => i.OrderId);
        builder.HasIndex(i => i.DishId);
    }
}
