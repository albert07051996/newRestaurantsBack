using FluentValidation;

namespace Product.Application.Features.Reservations.Commands.CreateReservation;

public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty().WithMessage("Customer phone is required")
            .Matches(@"^(\+995)?5\d{8}$").WithMessage("Invalid Georgian phone number format")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");

        RuleFor(x => x.ReservationDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Reservation date must be today or in the future");

        RuleFor(x => x.ReservationTime)
            .NotEmpty().WithMessage("Reservation time is required")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Invalid time format. Expected HH:mm");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0).WithMessage("Guest count must be at least 1");

        RuleFor(x => x.TableNumber)
            .GreaterThan(0).WithMessage("Table number must be at least 1");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.DishId)
                .NotEmpty().WithMessage("Dish ID is required");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1");
        });
    }
}
