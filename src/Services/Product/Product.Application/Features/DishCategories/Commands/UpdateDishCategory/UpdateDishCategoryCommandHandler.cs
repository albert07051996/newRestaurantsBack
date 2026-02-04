using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.DishCategories.Commands.UpdateDishCategory;

public class UpdateDishCategoryCommandHandler : IRequestHandler<UpdateDishCategoryCommand, Result<DishCategoryResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public UpdateDishCategoryCommandHandler(
        IProductRepository productRepository,
        ICloudinaryService cloudinaryService)
    {
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<DishCategoryResponseDto>> Handle(UpdateDishCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. შევამოწმოთ არსებობს თუ არა კატეგორია
        var category = await _productRepository.GetDishCategoryByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result<DishCategoryResponseDto>.Failure(new Error(
                "DishCategory.NotFound",
                $"კატეგორია ID {request.Id} ვერ მოიძებნა."
            ));
        }

        // 2. თუ ახალი სურათია მოწოდებული, ავტვირთოთ Cloudinary-ში
        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            // ვალიდაცია
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(request.ImageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Result<DishCategoryResponseDto>.Failure(new Error(
                    "Image.InvalidFormat",
                    "მხოლოდ .jpg, .jpeg, .png, .gif, .webp ფორმატები დაშვებულია."
                ));
            }

            if (request.ImageFile.Length > 5 * 1024 * 1024)
            {
                return Result<DishCategoryResponseDto>.Failure(new Error(
                    "Image.TooLarge",
                    "სურათის ზომა არ უნდა აღემატებოდეს 5MB-ს."
                ));
            }

            try
            {
                // წავშალოთ ძველი სურათი Cloudinary-დან (თუ არსებობს)
                if (!string.IsNullOrEmpty(category.ImagePublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(category.ImagePublicId, cancellationToken);
                }

                // ავტვირთოთ ახალი სურათი
                var publicId = $"dish-categories/{request.Id}";
                var imageUrl = await _cloudinaryService.UploadImageAsync(
                    request.ImageFile,
                    publicId,
                    cancellationToken
                );

                // განვაახლოთ სურათი Domain-ში
                category.UpdateImage(imageUrl, publicId);
            }
            catch (Exception ex)
            {
                return Result<DishCategoryResponseDto>.Failure(new Error(
                    "Image.UploadFailed",
                    $"სურათის ატვირთვა ვერ მოხერხდა: {ex.Message}"
                ));
            }
        }

        // 3. განვაახლოთ კატეგორია (Domain method)
        try
        {
            category.Update(
                nameKa: request.NameKa,
                nameEn: request.NameEn,
                descriptionKa: request.DescriptionKa,
                descriptionEn: request.DescriptionEn,
                displayOrder: request.DisplayOrder
            );

            // განვაახლოთ სტატუსი
            if (request.IsActive)
                category.Activate();
            else
                category.Deactivate();
        }
        catch (ArgumentException ex)
        {
            return Result<DishCategoryResponseDto>.Failure(new Error(
                "DishCategory.ValidationFailed",
                ex.Message
            ));
        }

        // 4. შევინახოთ ბაზაში
        await _productRepository.UpdateDishCategoryAsync(category, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 5. დავაბრუნოთ Response
        var response = MapToResponseDto(category);
        return Result<DishCategoryResponseDto>.Success(response);
    }

    private static DishCategoryResponseDto MapToResponseDto(Domain.Entities.DishCategory category)
    {
        return new DishCategoryResponseDto
        {
            Id = category.Id,
            NameKa = category.NameKa,
            NameEn = category.NameEn,
            DescriptionKa = category.DescriptionKa,
            DescriptionEn = category.DescriptionEn,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}
