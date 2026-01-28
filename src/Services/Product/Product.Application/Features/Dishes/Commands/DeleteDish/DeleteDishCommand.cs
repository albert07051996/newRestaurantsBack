using MediatR;
using Product.Application.Common.Models;

namespace Product.Application.Features.Dishes.Commands.DeleteDish;

/// <summary>
/// კერძის წაშლის Command (Soft Delete)
/// </summary>
public record DeleteDishCommand(Guid Id) : IRequest<Result<bool>>;