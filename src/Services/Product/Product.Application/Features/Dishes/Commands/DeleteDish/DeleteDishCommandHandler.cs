using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;

namespace Product.Application.Features.Dishes.Commands.DeleteDish;

/// <summary>
/// Handler for soft-deleting a dish.
/// </summary>
public sealed class DeleteDishCommandHandler : IRequestHandler<DeleteDishCommand, Result<bool>>
{
    private readonly IDishRepository _dishRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDishCommandHandler(IDishRepository dishRepository, IUnitOfWork unitOfWork)
    {
        _dishRepository = dishRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteDishCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate dish exists
        var dish = await _dishRepository.GetDishByIdAsync(request.Id, cancellationToken);
        if (dish is null)
        {
            return Result<bool>.Failure(new Error(
                "Dish.NotFound",
                $"Dish with ID {request.Id} was not found."
            ));
        }

        // 2. Soft delete (preserves image in Cloudinary for potential restore)
        await _dishRepository.DeleteDishAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
