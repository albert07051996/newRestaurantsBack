using FluentValidation;

namespace Product.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Validator for CreateOrderCommand.
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty().WithMessage("Customer phone is required")
            .Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Invalid phone number format")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");

        RuleFor(x => x.OrderType)
            .NotEmpty().WithMessage("Order type is required")
            .Must(BeValidOrderType).WithMessage("Invalid order type. Valid types are: DineIn, TakeAway, Delivery");

        RuleFor(x => x.CustomerAddress)
            .NotEmpty().When(x => x.OrderType.Equals("Delivery", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Address is required for delivery orders");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.DishId)
                .NotEmpty().WithMessage("Dish ID is required");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1");
        });
    }

    private static bool BeValidOrderType(string orderType)
    {
        return orderType.Equals("DineIn", StringComparison.OrdinalIgnoreCase) ||
               orderType.Equals("TakeAway", StringComparison.OrdinalIgnoreCase) ||
               orderType.Equals("Delivery", StringComparison.OrdinalIgnoreCase);
    }
}
