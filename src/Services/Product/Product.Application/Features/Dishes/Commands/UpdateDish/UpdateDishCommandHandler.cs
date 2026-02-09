using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Commands.UpdateDish;

/// <summary>
/// Handler for updating an existing dish.
/// </summary>
public sealed class UpdateDishCommandHandler : IRequestHandler<UpdateDishCommand, Result<DishResponseDto>>
{
    private readonly IDishRepository _dishRepository;
    private readonly IDishCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public UpdateDishCommandHandler(
        IDishRepository dishRepository,
        IDishCategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ICloudinaryService cloudinaryService)
    {
        _dishRepository = dishRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<DishResponseDto>> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate dish exists
        var dish = await _dishRepository.GetDishByIdAsync(request.Id, cancellationToken);
        if (dish is null)
        {
            return Result<DishResponseDto>.Failure(new Error(
                "Dish.NotFound",
                $"Dish with ID {request.Id} was not found."
            ));
        }

        // 2. Validate category exists
        var categoryExists = await _categoryRepository.DishCategoryExistsAsync(request.DishCategoryId, cancellationToken);
        if (!categoryExists)
        {
            return Result<DishResponseDto>.Failure(new Error(
                "DishCategory.NotFound",
                $"Category with ID {request.DishCategoryId} was not found."
            ));
        }

        // 3. Upload new image if provided
        if (request.ImageFile is { Length: > 0 })
        {
            var imageResult = await HandleImageUploadAsync(dish, request, cancellationToken);
            if (!imageResult.IsSuccess)
            {
                return Result<DishResponseDto>.Failure(imageResult.Error!);
            }
        }

        // 4. Update dish using domain method
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

            dish.SetAvailability(request.IsAvailable);
        }
        catch (ArgumentException ex)
        {
            return Result<DishResponseDto>.Failure(new Error("Dish.ValidationFailed", ex.Message));
        }

        // 5. Persist changes
        _dishRepository.UpdateDish(dish);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DishResponseDto>.Success(dish.ToDto());
    }

    private async Task<Result<bool>> HandleImageUploadAsync(
        Domain.Entities.Dish dish,
        UpdateDishCommand request,
        CancellationToken cancellationToken)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(request.ImageFile!.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return Result<bool>.Failure(new Error(
                "Image.InvalidFormat",
                "Only .jpg, .jpeg, .png, .gif, .webp formats are allowed."
            ));
        }

        if (request.ImageFile.Length > 5 * 1024 * 1024)
        {
            return Result<bool>.Failure(new Error(
                "Image.TooLarge",
                "Image size must not exceed 5MB."
            ));
        }

        try
        {
            // Delete old image from Cloudinary if exists
            if (!string.IsNullOrEmpty(dish.ImagePublicId))
            {
                await _cloudinaryService.DeleteImageAsync(dish.ImagePublicId, cancellationToken);
            }

            // Upload new image
            var publicId = $"dishes/{request.Id}";
            var imageUrl = await _cloudinaryService.UploadImageAsync(
                request.ImageFile,
                publicId,
                cancellationToken
            );

            dish.UpdateImage(imageUrl, publicId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(new Error(
                "Image.UploadFailed",
                $"Failed to upload image: {ex.Message}"
            ));
        }
    }
}
