using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Domain.Entities;

namespace Product.Persistence.Configurations;

public class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable("Dishes");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.NameKa)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.DescriptionKa)
            .HasMaxLength(1000);

        builder.Property(d => d.DescriptionEn)
            .HasMaxLength(1000);

        builder.Property(d => d.Price)
            .HasPrecision(18, 2);

        builder.Property(d => d.Ingredients)
            .HasMaxLength(500);

        builder.Property(d => d.IngredientsEn)
            .HasMaxLength(500);

        builder.Property(d => d.Volume)
            .HasMaxLength(50);

        builder.Property(d => d.AlcoholContent)
            .HasMaxLength(50);

        builder.Property(d => d.Comment)
            .HasMaxLength(500);

        builder.Property(d => d.ImageUrl)
            .HasMaxLength(500);

        builder.Property(d => d.VideoUrl)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired();

        // Relationship
        builder.HasOne(d => d.DishCategory)
            .WithMany(dc => dc.Dishes)
            .HasForeignKey(d => d.DishCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(d => d.DishCategoryId);
    }
}
