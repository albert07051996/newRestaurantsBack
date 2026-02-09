using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.Dishes.Commands.AddDish;

/// <summary>
/// Handler for adding a new dish.
/// </summary>
public sealed class AddDishCommandHandler : IRequestHandler<AddDishCommand, Result<DishResponseDto>>
{
    private readonly IDishRepository _dishRepository;
    private readonly IDishCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public AddDishCommandHandler(
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

    public async Task<Result<DishResponseDto>> Handle(AddDishCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate category exists
        var categoryExists = await _categoryRepository.DishCategoryExistsAsync(request.DishCategoryId, cancellationToken);
        if (!categoryExists)
        {
            return Result<DishResponseDto>.Failure(new Error(
                "DishCategory.NotFound",
                $"Category with ID {request.DishCategoryId} was not found."
            ));
        }

        // 2. Upload image to Cloudinary if provided
        string? imageUrl = null;
        string? imagePublicId = null;

        if (request.ImageFile is { Length: > 0 })
        {
            var uploadResult = await UploadImageAsync(request, cancellationToken);
            if (!uploadResult.IsSuccess)
            {
                return Result<DishResponseDto>.Failure(uploadResult.Error!);
            }
            (imageUrl, imagePublicId) = uploadResult.Value!;
        }

        // 3. Create Dish entity using factory method
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
            // Clean up uploaded image on validation failure
            await TryDeleteImageAsync(imagePublicId, cancellationToken);
            return Result<DishResponseDto>.Failure(new Error("Dish.ValidationFailed", ex.Message));
        }

        // 4. Persist to database
        await _dishRepository.AddDishAsync(dish, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return DTO
        return Result<DishResponseDto>.Success(dish.ToDto());
    }

    private async Task<Result<(string Url, string PublicId)>> UploadImageAsync(
        AddDishCommand request,
        CancellationToken cancellationToken)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(request.ImageFile!.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return Result<(string, string)>.Failure(new Error(
                "Image.InvalidFormat",
                "Only .jpg, .jpeg, .png, .gif, .webp formats are allowed."
            ));
        }

        if (request.ImageFile.Length > 5 * 1024 * 1024)
        {
            return Result<(string, string)>.Failure(new Error(
                "Image.TooLarge",
                "Image size must not exceed 5MB."
            ));
        }

        try
        {
            var publicId = $"dishes/{Guid.NewGuid()}";
            var imageUrl = await _cloudinaryService.UploadImageAsync(
                request.ImageFile,
                publicId,
                cancellationToken
            );
            return Result<(string, string)>.Success((imageUrl, publicId));
        }
        catch (Exception ex)
        {
            return Result<(string, string)>.Failure(new Error(
                "Image.UploadFailed",
                $"Failed to upload image: {ex.Message}"
            ));
        }
    }

    private async Task TryDeleteImageAsync(string? imagePublicId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(imagePublicId)) return;

        try
        {
            await _cloudinaryService.DeleteImageAsync(imagePublicId, cancellationToken);
        }
        catch
        {
            // Log error but don't fail the operation
        }
    }
}
