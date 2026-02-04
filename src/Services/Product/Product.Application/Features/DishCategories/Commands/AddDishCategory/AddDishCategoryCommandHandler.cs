using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;
using Product.Application.DTOs;
using Product.Domain.Entities;

namespace Product.Application.Features.DishCategories.Commands.AddDishCategory;

public class AddDishCategoryCommandHandler : IRequestHandler<AddDishCategoryCommand, Result<DishCategoryResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public AddDishCategoryCommandHandler(
        IProductRepository productRepository,
        ICloudinaryService cloudinaryService)
    {
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<DishCategoryResponseDto>> Handle(AddDishCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. თუ სურათია მიწოდებული, ავტვირთოთ Cloudinary-ში
        string? imageUrl = null;
        string? imagePublicId = null;

        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            // ვალიდაცია სურათის ტიპის
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(request.ImageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Result<DishCategoryResponseDto>.Failure(new Error(
                    "Image.InvalidFormat",
                    "მხოლოდ .jpg, .jpeg, .png, .gif, .webp ფორმატები დაშვებულია."
                ));
            }

            // შევამოწმოთ ფაილის ზომა (მაქსიმუმ 5MB)
            if (request.ImageFile.Length > 5 * 1024 * 1024)
            {
                return Result<DishCategoryResponseDto>.Failure(new Error(
                    "Image.TooLarge",
                    "სურათის ზომა არ უნდა აღემატებოდეს 5MB-ს."
                ));
            }

            try
            {
                // გავაკეთოთ უნიკალური Public ID
                var publicId = $"dish-categories/{Guid.NewGuid()}";

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
                return Result<DishCategoryResponseDto>.Failure(new Error(
                    "Image.UploadFailed",
                    $"სურათის ატვირთვა ვერ მოხერხდა: {ex.Message}"
                ));
            }
        }

        // 2. შევქმნათ DishCategory entity (Factory Method - Domain-ში validation)
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

            return Result<DishCategoryResponseDto>.Failure(new Error(
                "DishCategory.ValidationFailed",
                ex.Message
            ));
        }

        // 3. დავამატოთ ბაზაში
        await _productRepository.AddDishCategoryAsync(category, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 4. დავაბრუნოთ Response DTO
        var response = MapToResponseDto(category);
        return Result<DishCategoryResponseDto>.Success(response);
    }

    private static DishCategoryResponseDto MapToResponseDto(DishCategory category)
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
