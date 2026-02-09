using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;

namespace Product.Application.Features.DishCategories.Commands.DeleteDishCategory;

/// <summary>
/// Handler for soft-deleting a dish category.
/// </summary>
public sealed class DeleteDishCategoryCommandHandler : IRequestHandler<DeleteDishCategoryCommand, Result<bool>>
{
    private readonly IDishCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDishCategoryCommandHandler(IDishCategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteDishCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate category exists
        var category = await _categoryRepository.GetDishCategoryByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result<bool>.Failure(new Error(
                "DishCategory.NotFound",
                $"Category with ID {request.Id} was not found."
            ));
        }

        // 2. Soft delete (preserves image in Cloudinary for potential restore)
        await _categoryRepository.DeleteDishCategoryAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
