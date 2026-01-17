using FluentValidation;
using RestaurantApp.Application.DTOs.Order;

namespace RestaurantApp.Application.Validators.Order;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.BranchId)
            .GreaterThan(0)
            .WithMessage("Valid branch must be selected");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must contain at least one item")
            .Must(items => items != null && items.Count > 0)
            .WithMessage("Order must contain at least one item");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemDtoValidator());

        When(x => x.OrderType == Domain.Enums.OrderType.Delivery, () =>
        {
            RuleFor(x => x.DeliveryAddressLine)
                .NotEmpty()
                .WithMessage("Delivery address is required for delivery orders")
                .MaximumLength(500)
                .WithMessage("Delivery address must not exceed 500 characters");

            RuleFor(x => x.DeliveryLatitude)
                .InclusiveBetween(-90, 90)
                .When(x => x.DeliveryLatitude.HasValue)
                .WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.DeliveryLongitude)
                .InclusiveBetween(-180, 180)
                .When(x => x.DeliveryLongitude.HasValue)
                .WithMessage("Longitude must be between -180 and 180");
        });

        RuleFor(x => x.CouponCode)
            .MaximumLength(50)
            .Matches("^[A-Z0-9-]*$")
            .When(x => !string.IsNullOrEmpty(x.CouponCode))
            .WithMessage("Coupon code can only contain uppercase letters, numbers, and hyphens");

        RuleFor(x => x.CustomerNotes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.CustomerNotes))
            .WithMessage("Customer notes must not exceed 1000 characters");

        RuleFor(x => x.DeliveryNotes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.DeliveryNotes))
            .WithMessage("Delivery notes must not exceed 500 characters");
    }
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.MenuItemId)
            .GreaterThan(0)
            .WithMessage("Valid menu item must be selected");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 99)
            .WithMessage("Quantity must be between 1 and 99");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Item notes must not exceed 500 characters");
    }
}
