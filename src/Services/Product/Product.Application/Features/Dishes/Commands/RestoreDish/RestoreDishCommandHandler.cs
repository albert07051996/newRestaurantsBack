using MediatR;
using Product.Application.Common.Interfaces;
using Product.Application.Common.Models;

namespace Product.Application.Features.Dishes.Commands.RestoreDish;

public class RestoreDishCommandHandler : IRequestHandler<RestoreDishCommand, Result<bool>>
{
    private readonly IProductRepository _productRepository;

    public RestoreDishCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<bool>> Handle(RestoreDishCommand request, CancellationToken cancellationToken)
    {
        // აღვადგინოთ კერძი (Domain method)
        await _productRepository.RestoreDishAsync(request.Id, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}