using Product.Domain.Common;

namespace Product.Domain.Entities;

/// <summary>
/// კერძის კატეგორია
/// </summary>
public class DishCategory : BaseEntity
{
    public string NameKa { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public string? DescriptionKa { get; private set; }
    public string? DescriptionEn { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation Properties
    private readonly List<Dish> _dishes = new();
    public IReadOnlyCollection<Dish> Dishes => _dishes.AsReadOnly();

    // Private constructor for EF Core
    private DishCategory() : base() { }

    // Factory Method
    public static DishCategory Create(
        string nameKa,
        string nameEn,
        string? descriptionKa = null,
        string? descriptionEn = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(nameKa))
            throw new ArgumentException("ქართული სახელი აუცილებელია", nameof(nameKa));

        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("ინგლისური სახელი აუცილებელია", nameof(nameEn));

        var category = new DishCategory
        {
            NameKa = nameKa,
            NameEn = nameEn,
            DescriptionKa = descriptionKa,
            DescriptionEn = descriptionEn,
            DisplayOrder = displayOrder,
            IsActive = true
        };

        return category;
    }

    // Update Method
    public void Update(
        string nameKa,
        string nameEn,
        string? descriptionKa,
        string? descriptionEn,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(nameKa))
            throw new ArgumentException("ქართული სახელი აუცილებელია", nameof(nameKa));

        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("ინგლისური სახელი აუცილებელია", nameof(nameEn));

        NameKa = nameKa;
        NameEn = nameEn;
        DescriptionKa = descriptionKa;
        DescriptionEn = descriptionEn;
        DisplayOrder = displayOrder;

        UpdateTimestamp();
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void ChangeDisplayOrder(int newOrder)
    {
        DisplayOrder = newOrder;
        UpdateTimestamp();
    }
}