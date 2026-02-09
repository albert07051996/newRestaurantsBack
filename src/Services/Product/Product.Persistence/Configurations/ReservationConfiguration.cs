using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Entities;

namespace Product.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReservationNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(r => r.ReservationNumber)
            .IsUnique();

        builder.Property(r => r.CustomerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.ReservationDate)
            .IsRequired();

        builder.Property(r => r.ReservationTime)
            .IsRequired();

        builder.Property(r => r.GuestCount)
            .IsRequired();

        builder.Property(r => r.TableNumber)
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.ReservationDate);
        builder.HasIndex(r => r.CreatedAt);

        builder.Navigation(r => r.ReservationItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(r => r.ReservationItems)
            .WithOne(i => i.Reservation)
            .HasForeignKey(i => i.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
