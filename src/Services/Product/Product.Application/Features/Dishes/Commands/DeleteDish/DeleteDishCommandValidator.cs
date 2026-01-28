using FluentValidation;

namespace Product.Application.Features.Dishes.Commands.DeleteDish;

public class DeleteDishCommandValidator : AbstractValidator<DeleteDishCommand>
{
	public DeleteDishCommandValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("ID აუცილებელია");
	}
}