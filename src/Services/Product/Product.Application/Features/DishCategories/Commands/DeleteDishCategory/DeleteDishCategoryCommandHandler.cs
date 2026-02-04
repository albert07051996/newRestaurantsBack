using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;

namespace Product.Application.Features.DishCategories.Commands.DeleteDishCategory;

public class DeleteDishCategoryCommandHandler : IRequestHandler<DeleteDishCategoryCommand, Result<bool>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public DeleteDishCategoryCommandHandler(
        IProductRepository productRepository,
        ICloudinaryService cloudinaryService)
    {
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<bool>> Handle(DeleteDishCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. მოვიძიოთ კატეგორია
        var category = await _productRepository.GetDishCategoryByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result<bool>.Failure(new Error(
                "DishCategory.NotFound",
                $"კატეგორია ID {request.Id} ვერ მოიძებნა."
            ));
        }

        // 2. Soft Delete (Domain method)
        await _productRepository.DeleteDishCategoryAsync(request.Id, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 3. (Optional) წავშალოთ სურათი Cloudinary-დან
        // შენიშვნა: შეგიძლია დატოვო Cloudinary-ში თუ გინდა Restore-ისას სურათიც დაბრუნდეს
        /*
        if (!string.IsNullOrEmpty(category.ImagePublicId))
        {
            try
            {
                await _cloudinaryService.DeleteImageAsync(category.ImagePublicId, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the operation
            }
        }
        */

        return Result<bool>.Success(true);
    }
}
