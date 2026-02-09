using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.DishCategories.Commands.UpdateDishCategory;

/// <summary>
/// Handler for updating an existing dish category.
/// </summary>
public sealed class UpdateDishCategoryCommandHandler : IRequestHandler<UpdateDishCategoryCommand, Result<DishCategoryResponseDto>>
{
    private readonly IDishCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public UpdateDishCategoryCommandHandler(
        IDishCategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ICloudinaryService cloudinaryService)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<DishCategoryResponseDto>> Handle(UpdateDishCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate category exists
        var category = await _categoryRepository.GetDishCategoryByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result<DishCategoryResponseDto>.Failure(new Error(
                "DishCategory.NotFound",
                $"Category with ID {request.Id} was not found."
            ));
        }

        // 2. Upload new image if provided
        if (request.ImageFile is { Length: > 0 })
        {
            var imageResult = await HandleImageUploadAsync(category, request, cancellationToken);
            if (!imageResult.IsSuccess)
            {
                return Result<DishCategoryResponseDto>.Failure(imageResult.Error!);
            }
        }

        // 3. Update category using domain method
        try
        {
            category.Update(
                nameKa: request.NameKa,
                nameEn: request.NameEn,
                descriptionKa: request.DescriptionKa,
                descriptionEn: request.DescriptionEn,
                displayOrder: request.DisplayOrder
            );

            if (request.IsActive)
                category.Activate();
            else
                category.Deactivate();
        }
        catch (ArgumentException ex)
        {
            return Result<DishCategoryResponseDto>.Failure(new Error("DishCategory.ValidationFailed", ex.Message));
        }

        // 4. Persist changes
        _categoryRepository.UpdateDishCategory(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DishCategoryResponseDto>.Success(category.ToDto());
    }

    private async Task<Result<bool>> HandleImageUploadAsync(
        Domain.Entities.DishCategory category,
        UpdateDishCategoryCommand request,
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
            if (!string.IsNullOrEmpty(category.ImagePublicId))
            {
                await _cloudinaryService.DeleteImageAsync(category.ImagePublicId, cancellationToken);
            }

            // Upload new image
            var publicId = $"dish-categories/{request.Id}";
            var imageUrl = await _cloudinaryService.UploadImageAsync(
                request.ImageFile,
                publicId,
                cancellationToken
            );

            category.UpdateImage(imageUrl, publicId);
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
