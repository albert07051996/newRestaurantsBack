using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;

namespace Product.Application.Features.Dishes.Commands.RestoreDish;

/// <summary>
/// Handler for restoring a soft-deleted dish.
/// </summary>
public sealed class RestoreDishCommandHandler : IRequestHandler<RestoreDishCommand, Result<bool>>
{
    private readonly IDishRepository _dishRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RestoreDishCommandHandler(IDishRepository dishRepository, IUnitOfWork unitOfWork)
    {
        _dishRepository = dishRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RestoreDishCommand request, CancellationToken cancellationToken)
    {
        await _dishRepository.RestoreDishAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
