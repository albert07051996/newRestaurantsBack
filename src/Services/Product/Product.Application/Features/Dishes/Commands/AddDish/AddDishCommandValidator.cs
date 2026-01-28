using FluentValidation;

namespace Product.Application.Features.Dishes.Commands.AddDish;

/// <summary>
/// AddDishCommand-ის ვალიდატორი
/// </summary>
public class AddDishCommandValidator : AbstractValidator<AddDishCommand>
{
    public AddDishCommandValidator()
    {
        RuleFor(x => x.NameKa)
            .NotEmpty().WithMessage("ქართული სახელი აუცილებელია")
            .MaximumLength(200).WithMessage("ქართული სახელი არ უნდა აღემატებოდეს 200 სიმბოლოს");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("ინგლისური სახელი აუცილებელია")
            .MaximumLength(200).WithMessage("ინგლისური სახელი არ უნდა აღემატებოდეს 200 სიმბოლოს");

        RuleFor(x => x.DescriptionKa)
            .MaximumLength(1000).WithMessage("ქართული აღწერა არ უნდა აღემატებოდეს 1000 სიმბოლოს");

        RuleFor(x => x.DescriptionEn)
            .MaximumLength(1000).WithMessage("ინგლისური აღწერა არ უნდა აღემატებოდეს 1000 სიმბოლოს");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).When(x => x.Price.HasValue)
            .WithMessage("ფასი უნდა იყოს დადებითი რიცხვი");

        RuleFor(x => x.DishCategoryId)
            .NotEmpty().WithMessage("კატეგორია აუცილებელია");

        RuleFor(x => x.PreparationTimeMinutes)
            .GreaterThanOrEqualTo(0).When(x => x.PreparationTimeMinutes.HasValue)
            .WithMessage("მომზადების დრო უნდა იყოს დადებითი რიცხვი");

        RuleFor(x => x.Calories)
            .GreaterThanOrEqualTo(0).When(x => x.Calories.HasValue)
            .WithMessage("კალორია უნდა იყოს დადებითი რიცხვი");

        RuleFor(x => x.SpicyLevel)
            .InclusiveBetween(0, 5).When(x => x.SpicyLevel.HasValue)
            .WithMessage("სიცხარის დონე უნდა იყოს 0-დან 5-მდე");

        RuleFor(x => x.Ingredients)
            .MaximumLength(500).WithMessage("ქართული ინგრედიენტები არ უნდა აღემატებოდეს 500 სიმბოლოს");

        RuleFor(x => x.IngredientsEn)
            .MaximumLength(500).WithMessage("ინგლისური ინგრედიენტები არ უნდა აღემატებოდეს 500 სიმბოლოს");

        RuleFor(x => x.ImageFile)
            .Must(BeValidImageFile).When(x => x.ImageFile != null)
            .WithMessage("სურათის ფორმატი უნდა იყოს .jpg, .jpeg, .png, .gif, .webp");

        RuleFor(x => x.ImageFile)
            .Must(BeValidFileSize).When(x => x.ImageFile != null)
            .WithMessage("სურათის ზომა არ უნდა აღემატებოდეს 5MB");

        RuleFor(x => x.VideoUrl)
            .Must(BeValidUrl).When(x => !string.IsNullOrWhiteSpace(x.VideoUrl))
            .WithMessage("ვიდეოს URL არასწორია");
    }

    private bool BeValidImageFile(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        return allowedExtensions.Contains(fileExtension);
    }

    private bool BeValidFileSize(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return true;
        
        // 5MB = 5 * 1024 * 1024 bytes
        return file.Length <= 5 * 1024 * 1024;
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
