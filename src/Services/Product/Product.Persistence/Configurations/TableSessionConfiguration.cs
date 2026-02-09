using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Entities;

namespace Product.Persistence.Configurations;

public class TableSessionConfiguration : IEntityTypeConfiguration<TableSession>
{
    public void Configure(EntityTypeBuilder<TableSession> builder)
    {
        builder.ToTable("TableSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SessionNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(s => s.SessionNumber)
            .IsUnique();

        builder.Property(s => s.CustomerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.TableNumber);
        builder.HasIndex(s => s.CreatedAt);

        builder.Navigation(s => s.Orders)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.Orders)
            .WithOne(o => o.TableSession)
            .HasForeignKey(o => o.TableSessionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
