using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Entities;

namespace Product.Persistence.Configurations;

public class DishCategoryConfiguration : IEntityTypeConfiguration<DishCategory>
{
    public void Configure(EntityTypeBuilder<DishCategory> builder)
    {
        builder.ToTable("DishCategories");

        builder.HasKey(dc => dc.Id);

        builder.Property(dc => dc.NameKa)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(dc => dc.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(dc => dc.DescriptionKa)
            .HasMaxLength(500);

        builder.Property(dc => dc.DescriptionEn)
            .HasMaxLength(500);

        builder.Property(dc => dc.DisplayOrder)
            .IsRequired();

        builder.Property(dc => dc.IsActive)
            .IsRequired();

        builder.Property(dc => dc.CreatedAt)
            .IsRequired();

        builder.Property(dc => dc.UpdatedAt)
            .IsRequired();
    }
}
