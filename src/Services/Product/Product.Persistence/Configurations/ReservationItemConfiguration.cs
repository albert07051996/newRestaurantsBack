using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Entities;

namespace Product.Persistence.Configurations;

public class ReservationItemConfiguration : IEntityTypeConfiguration<ReservationItem>
{
    public void Configure(EntityTypeBuilder<ReservationItem> builder)
    {
        builder.ToTable("ReservationItems");

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

        builder.HasIndex(i => i.ReservationId);
        builder.HasIndex(i => i.DishId);
    }
}
