using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;

namespace Product.Application.Features.Dishes.Commands.DeleteDish;

public class DeleteDishCommandHandler : IRequestHandler<DeleteDishCommand, Result<bool>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public DeleteDishCommandHandler(
        IProductRepository productRepository,
        ICloudinaryService cloudinaryService)
    {
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result<bool>> Handle(DeleteDishCommand request, CancellationToken cancellationToken)
    {
        // 1. მოვიძიოთ კერძი
        var dish = await _productRepository.GetDishByIdAsync(request.Id, cancellationToken);
        if (dish == null)
        {
            return Result<bool>.Failure(new Error(
                "Dish.NotFound",
                $"კერძი ID {request.Id} ვერ მოიძებნა."
            ));
        }

        // 2. Soft Delete (Domain method)
        await _productRepository.DeleteDishAsync(request.Id, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 3. (Optional) წავშალოთ სურათი Cloudinary-დან
        // შენიშვნა: შეგიძლია დატოვო Cloudinary-ში თუ გინდა Restore-ისას სურათიც დაბრუნდეს
        /*
        if (!string.IsNullOrEmpty(dish.ImagePublicId))
        {
            try
            {
                await _cloudinaryService.DeleteImageAsync(dish.ImagePublicId, cancellationToken);
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