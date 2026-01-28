using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Commands.UpdateDish;

public class UpdateDishCommandHandler : IRequestHandler<UpdateDishCommand, Result<DishResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public UpdateDishCommandHandler(
        IProductRepository productRepository,
        ICloudinaryService cloudinaryService)
    {
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<DishResponseDto>> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        // 1. შევამოწმოთ არსებობს თუ არა კერძი
        var dish = await _productRepository.GetDishByIdAsync(request.Id, cancellationToken);
        if (dish == null)
        {
            return Result<DishResponseDto>.Failure(new Error(
                "Dish.NotFound",
                $"კერძი ID {request.Id} ვერ მოიძებნა."
            ));
        }

        // 2. შევამოწმოთ არსებობს თუ არა კატეგორია
        var categoryExists = await _productRepository.DishCategoryExistsAsync(request.DishCategoryId, cancellationToken);
        if (!categoryExists)
        {
            return Result<DishResponseDto>.Failure(new Error(
                "DishCategory.NotFound",
                $"კატეგორია ID {request.DishCategoryId} ვერ მოიძებნა."
            ));
        }

        // 3. თუ ახალი სურათია მოწოდებული, ავტვირთოთ Cloudinary-ში
        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            // ვალიდაცია
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(request.ImageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Result<DishResponseDto>.Failure(new Error(
                    "Image.InvalidFormat",
                    "მხოლოდ .jpg, .jpeg, .png, .gif, .webp ფორმატები დაშვებულია."
                ));
            }

            if (request.ImageFile.Length > 5 * 1024 * 1024)
            {
                return Result<DishResponseDto>.Failure(new Error(
                    "Image.TooLarge",
                    "სურათის ზომა არ უნდა აღემატებოდეს 5MB-ს."
                ));
            }

            try
            {
                // წავშალოთ ძველი სურათი Cloudinary-დან (თუ არსებობს)
                if (!string.IsNullOrEmpty(dish.ImagePublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(dish.ImagePublicId, cancellationToken);
                }

                // ავტვირთოთ ახალი სურათი
                var imageUrl = await _cloudinaryService.UploadImageAsync(
                    request.ImageFile,
                    $"dishes/{request.Id}",
                    cancellationToken
                );

                // განვაახლოთ სურათი Domain-ში
                dish.UpdateImage(imageUrl, $"dishes/{request.Id}");
            }
            catch (Exception ex)
            {
                return Result<DishResponseDto>.Failure(new Error(
                    "Image.UploadFailed",
                    $"სურათის ატვირთვა ვერ მოხერხდა: {ex.Message}"
                ));
            }
        }

        // 4. განვაახლოთ კერძი (Domain method)
        try
        {
            dish.Update(
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
                videoUrl: request.VideoUrl
            );

            // განვაახლოთ ხელმისაწვდომობა
            dish.SetAvailability(request.IsAvailable);
        }
        catch (ArgumentException ex)
        {
            return Result<DishResponseDto>.Failure(new Error(
                "Dish.ValidationFailed",
                ex.Message
            ));
        }

        // 5. შევინახოთ ბაზაში
        await _productRepository.UpdateDishAsync(dish, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 6. დავაბრუნოთ Response
        var response = MapToDishResponseDto(dish);
        return Result<DishResponseDto>.Success(response);
    }

    private static DishResponseDto MapToDishResponseDto(Domain.Entities.Dish dish)
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