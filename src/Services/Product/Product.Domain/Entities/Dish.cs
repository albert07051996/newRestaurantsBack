using Product.Domain.Common;

namespace Product.Domain.Entities;

/// <summary>
/// კერძის Entity - Rich Domain Model
/// </summary>
public class Dish : BaseEntity
{
    // Properties
    public string NameKa { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public string DescriptionKa { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;
    public decimal? Price { get; private set; }
    public Guid DishCategoryId { get; private set; }
    public int? PreparationTimeMinutes { get; private set; }
    public int? Calories { get; private set; }
    public int? SpicyLevel { get; private set; }
    public string Ingredients { get; private set; } = string.Empty;
    public string IngredientsEn { get; private set; } = string.Empty;
    public string Volume { get; private set; } = string.Empty;
    public string AlcoholContent { get; private set; } = string.Empty;
    public bool IsVeganDish { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public string Comment { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public string? ImagePublicId { get; private set; } // Cloudinary Public ID
    public string? VideoUrl { get; private set; }

    // Navigation Properties
    public DishCategory? DishCategory { get; private set; }

    // Private constructor for EF Core
    private Dish() : base() { }

    // Factory Method - ახალი კერძის შესაქმნელად
    public static Dish Create(
        string nameKa,
        string nameEn,
        string descriptionKa,
        string descriptionEn,
        decimal? price,
        Guid dishCategoryId,
        int? preparationTimeMinutes = null,
        int? calories = null,
        int? spicyLevel = null,
        string? ingredients = null,
        string? ingredientsEn = null,
        string? volume = null,
        string? alcoholContent = null,
        bool isVeganDish = false,
        string? comment = null,
        string? imageUrl = null,
        string? imagePublicId = null,
        string? videoUrl = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(nameKa))
            throw new ArgumentException("ქართული სახელი აუცილებელია", nameof(nameKa));

        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("ინგლისური სახელი აუცილებელია", nameof(nameEn));

        if (dishCategoryId == Guid.Empty)
            throw new ArgumentException("კატეგორია აუცილებელია", nameof(dishCategoryId));

        if (spicyLevel.HasValue && (spicyLevel < 0 || spicyLevel > 5))
            throw new ArgumentException("სიცხარის დონე უნდა იყოს 0-დან 5-მდე", nameof(spicyLevel));

        var dish = new Dish
        {
            NameKa = nameKa,
            NameEn = nameEn,
            DescriptionKa = descriptionKa ?? string.Empty,
            DescriptionEn = descriptionEn ?? string.Empty,
            Price = price,
            DishCategoryId = dishCategoryId,
            PreparationTimeMinutes = preparationTimeMinutes,
            Calories = calories,
            SpicyLevel = spicyLevel,
            Ingredients = ingredients ?? string.Empty,
            IngredientsEn = ingredientsEn ?? string.Empty,
            Volume = volume ?? string.Empty,
            AlcoholContent = alcoholContent ?? string.Empty,
            IsVeganDish = isVeganDish,
            IsAvailable = true,
            Comment = comment ?? string.Empty,
            ImageUrl = imageUrl,
            ImagePublicId = imagePublicId,
            VideoUrl = videoUrl
        };

        return dish;
    }

    // Update Method - კერძის განახლებისთვის
    public void Update(
        string nameKa,
        string nameEn,
        string descriptionKa,
        string descriptionEn,
        decimal? price,
        Guid dishCategoryId,
        int? preparationTimeMinutes = null,
        int? calories = null,
        int? spicyLevel = null,
        string? ingredients = null,
        string? ingredientsEn = null,
        string? volume = null,
        string? alcoholContent = null,
        bool isVeganDish = false,
        string? comment = null,
        string? videoUrl = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(nameKa))
            throw new ArgumentException("ქართული სახელი აუცილებელია", nameof(nameKa));

        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("ინგლისური სახელი აუცილებელია", nameof(nameEn));

        if (dishCategoryId == Guid.Empty)
            throw new ArgumentException("კატეგორია აუცილებელია", nameof(dishCategoryId));

        if (spicyLevel.HasValue && (spicyLevel < 0 || spicyLevel > 5))
            throw new ArgumentException("სიცხარის დონე უნდა იყოს 0-დან 5-მდე", nameof(spicyLevel));

        NameKa = nameKa;
        NameEn = nameEn;
        DescriptionKa = descriptionKa ?? string.Empty;
        DescriptionEn = descriptionEn ?? string.Empty;
        Price = price;
        DishCategoryId = dishCategoryId;
        PreparationTimeMinutes = preparationTimeMinutes;
        Calories = calories;
        SpicyLevel = spicyLevel;
        Ingredients = ingredients ?? string.Empty;
        IngredientsEn = ingredientsEn ?? string.Empty;
        Volume = volume ?? string.Empty;
        AlcoholContent = alcoholContent ?? string.Empty;
        IsVeganDish = isVeganDish;
        Comment = comment ?? string.Empty;
        VideoUrl = videoUrl;

        UpdateTimestamp();
    }

    // სურათის განახლება
    public void UpdateImage(string imageUrl, string? imagePublicId = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("სურათის URL აუცილებელია", nameof(imageUrl));

        ImageUrl = imageUrl;
        ImagePublicId = imagePublicId;
        UpdateTimestamp();
    }

    // სურათის წაშლა
    public void RemoveImage()
    {
        ImageUrl = null;
        ImagePublicId = null;
        UpdateTimestamp();
    }

    // ხელმისაწვდომობის შეცვლა
    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        UpdateTimestamp();
    }

    // კერძის გააქტიურება
    public void Activate()
    {
        IsAvailable = true;
        UpdateTimestamp();
    }

    // კერძის დეაქტივაცია
    public void Deactivate()
    {
        IsAvailable = false;
        UpdateTimestamp();
    }
}