using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Mappings;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.DishCategories.Commands.AddDishCategory;

/// <summary>
/// Handler for adding a new dish category.
/// </summary>
public sealed class AddDishCategoryCommandHandler : IRequestHandler<AddDishCategoryCommand, Result<DishCategoryResponseDto>>
{
    private readonly IDishCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public AddDishCategoryCommandHandler(
        IDishCategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ICloudinaryService cloudinaryService)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<DishCategoryResponseDto>> Handle(AddDishCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. Upload image to Cloudinary if provided
        string? imageUrl = null;
        string? imagePublicId = null;

        if (request.ImageFile is { Length: > 0 })
        {
            var uploadResult = await UploadImageAsync(request, cancellationToken);
            if (!uploadResult.IsSuccess)
            {
                return Result<DishCategoryResponseDto>.Failure(uploadResult.Error!);
            }
            (imageUrl, imagePublicId) = uploadResult.Value!;
        }

        // 2. Create DishCategory entity using factory method
        DishCategory category;
        try
        {
            category = DishCategory.Create(
                nameKa: request.NameKa,
                nameEn: request.NameEn,
                descriptionKa: request.DescriptionKa,
                descriptionEn: request.DescriptionEn,
                displayOrder: request.DisplayOrder,
                imageUrl: imageUrl,
                imagePublicId: imagePublicId
            );
        }
        catch (ArgumentException ex)
        {
            await TryDeleteImageAsync(imagePublicId, cancellationToken);
            return Result<DishCategoryResponseDto>.Failure(new Error("DishCategory.ValidationFailed", ex.Message));
        }

        // 3. Persist to database
        await _categoryRepository.AddDishCategoryAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DishCategoryResponseDto>.Success(category.ToDto());
    }

    private async Task<Result<(string Url, string PublicId)>> UploadImageAsync(
        AddDishCategoryCommand request,
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
            var publicId = $"dish-categories/{Guid.NewGuid()}";
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
