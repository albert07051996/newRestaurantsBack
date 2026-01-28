using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.Dishes.Commands.AddDish;

public class AddDishCommandHandler : IRequestHandler<AddDishCommand, Result<DishResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public AddDishCommandHandler(
        IProductRepository productRepository,
        ICloudinaryService cloudinaryService)
    {
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<DishResponseDto>> Handle(AddDishCommand request, CancellationToken cancellationToken)
    {
        // 1. შევამოწმოთ არსებობს თუ არა DishCategory
        var categoryExists = await _productRepository.DishCategoryExistsAsync(request.DishCategoryId, cancellationToken);
        if (!categoryExists)
        {
            return Result<DishResponseDto>.Failure(new Error(
                "DishCategory.NotFound",
                $"კატეგორია ID {request.DishCategoryId} ვერ მოიძებნა."
            ));
        }

        // 2. თუ სურათია მიწოდებული, ავტვირთოთ Cloudinary-ში
        string? imageUrl = null;
        string? imagePublicId = null;

        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            // ვალიდაცია სურათის ტიპის
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(request.ImageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Result<DishResponseDto>.Failure(new Error(
                    "Image.InvalidFormat",
                    "მხოლოდ .jpg, .jpeg, .png, .gif, .webp ფორმატები დაშვებულია."
                ));
            }

            // შევამოწმოთ ფაილის ზომა (მაქსიმუმ 5MB)
            if (request.ImageFile.Length > 5 * 1024 * 1024)
            {
                return Result<DishResponseDto>.Failure(new Error(
                    "Image.TooLarge",
                    "სურათის ზომა არ უნდა აღემატებოდეს 5MB-ს."
                ));
            }

            try
            {
                // გავაკეთოთ უნიკალური Public ID
                var publicId = $"dishes/{Guid.NewGuid()}";

                // ავტვირთოთ სურათი Cloudinary-ში
                imageUrl = await _cloudinaryService.UploadImageAsync(
                    request.ImageFile,
                    publicId,
                    cancellationToken
                );

                imagePublicId = publicId;
            }
            catch (Exception ex)
            {
                return Result<DishResponseDto>.Failure(new Error(
                    "Image.UploadFailed",
                    $"სურათის ატვირთვა ვერ მოხერხდა: {ex.Message}"
                ));
            }
        }

        // 3. შევქმნათ Dish entity (Factory Method - Domain-ში validation)
        Dish dish;
        try
        {
            dish = Dish.Create(
                nameKa: request.NameKa,
                nameEn: request.NameEn,
                descriptionKa: request.DescriptionKa,
                descriptionEn: request.DescriptionEn,
                price: request.Price,
                dishCategoryId: request.DishCategoryId,
                preparationTimeMinutes: request.PreparationTimeMinutes,
                calories: request.Calories,
                spicyLevel: request.SpicyLevel,
                ingredients: request.Ingredients,
                ingredientsEn: request.IngredientsEn,
                volume: request.Volume,
                alcoholContent: request.AlcoholContent,
                isVeganDish: request.IsVeganDish,
                comment: request.Comment,
                imageUrl: imageUrl,
                imagePublicId: imagePublicId,
                videoUrl: request.VideoUrl
            );
        }
        catch (ArgumentException ex)
        {
            // თუ Domain Validation ჩაიშალა
            // და სურათი ატვირთული იყო, წავშალოთ Cloudinary-დან
            if (!string.IsNullOrEmpty(imagePublicId))
            {
                try
                {
                    await _cloudinaryService.DeleteImageAsync(imagePublicId, cancellationToken);
                }
                catch
                {
                    // Log error but don't fail
                }
            }

            return Result<DishResponseDto>.Failure(new Error(
                "Dish.ValidationFailed",
                ex.Message
            ));
        }

        // 4. დავამატოთ ბაზაში
        await _productRepository.AddDishAsync(dish, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 5. დავაბრუნოთ Response DTO
        var response = MapToDishResponseDto(dish);
        return Result<DishResponseDto>.Success(response);
    }

    private static DishResponseDto MapToDishResponseDto(Dish dish)
    {
        return new DishResponseDto
        {
            Id = dish.Id,
            NameKa = dish.NameKa,
            NameEn = dish.NameEn,
            DescriptionKa = dish.DescriptionKa,
            DescriptionEn = dish.DescriptionEn,
            Price = dish.Price,
            DishCategoryId = dish.DishCategoryId,
            PreparationTimeMinutes = dish.PreparationTimeMinutes,
            Calories = dish.Calories,
            SpicyLevel = dish.SpicyLevel,
            Ingredients = dish.Ingredients,
            IngredientsEn = dish.IngredientsEn,
            Volume = dish.Volume,
            AlcoholContent = dish.AlcoholContent,
            IsVeganDish = dish.IsVeganDish,
            Comment = dish.Comment,
            ImageUrl = dish.ImageUrl,
            VideoUrl = dish.VideoUrl,
            CreatedAt = dish.CreatedAt,
            UpdatedAt = dish.UpdatedAt
        };
    }
}