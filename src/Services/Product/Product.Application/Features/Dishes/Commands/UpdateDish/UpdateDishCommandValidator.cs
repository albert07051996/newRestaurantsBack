using FluentValidation;

namespace Product.Application.Features.Dishes.Commands.UpdateDish;

public class UpdateDishCommandValidator : AbstractValidator<UpdateDishCommand>
{
    public UpdateDishCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID აუცილებელია");

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

        RuleFor(x => x.SpicyLevel)
            .InclusiveBetween(0, 5).When(x => x.SpicyLevel.HasValue)
            .WithMessage("სიცხარის დონე უნდა იყოს 0-დან 5-მდე");

        RuleFor(x => x.ImageFile)
            .Must(BeValidImageFile).When(x => x.ImageFile != null)
            .WithMessage("სურათის ფორმატი უნდა იყოს .jpg, .jpeg, .png, .gif, .webp");

        RuleFor(x => x.ImageFile)
            .Must(BeValidFileSize).When(x => x.ImageFile != null)
            .WithMessage("სურათის ზომა არ უნდა აღემატებოდეს 5MB");
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
        return file.Length <= 5 * 1024 * 1024;
    }
}