using MediatR;
using Product.Application.Common.Models;

namespace Product.Application.Features.Dishes.Commands.RestoreDish;

/// <summary>
/// წაშლილი კერძის აღდგენის Command
/// </summary>
public record RestoreDishCommand(Guid Id) : IRequest<Result<bool>>;