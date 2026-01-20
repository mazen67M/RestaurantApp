using FluentValidation;
using RestaurantApp.Application.DTOs.Restaurant;

namespace RestaurantApp.Application.Validators.Restaurant;

public class CreateBranchDtoValidator : AbstractValidator<CreateBranchDto>
{
    public CreateBranchDtoValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required")
            .MaximumLength(200).WithMessage("Arabic name cannot exceed 200 characters");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required")
            .MaximumLength(200).WithMessage("English name cannot exceed 200 characters");

        RuleFor(x => x.AddressAr)
            .NotEmpty().WithMessage("Arabic address is required")
            .MaximumLength(500).WithMessage("Arabic address cannot exceed 500 characters");

        RuleFor(x => x.AddressEn)
            .NotEmpty().WithMessage("English address is required")
            .MaximumLength(500).WithMessage("English address cannot exceed 500 characters");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.DeliveryRadiusKm)
            .GreaterThan(0).WithMessage("Delivery radius must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Delivery radius cannot exceed 100 km");

        RuleFor(x => x.MinOrderAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum order amount cannot be negative")
            .LessThanOrEqualTo(1000).WithMessage("Minimum order amount cannot exceed 1000");

        RuleFor(x => x.DeliveryFee)
            .GreaterThanOrEqualTo(0).WithMessage("Delivery fee cannot be negative")
            .LessThanOrEqualTo(100).WithMessage("Delivery fee cannot exceed 100");

        RuleFor(x => x.FreeDeliveryThreshold)
            .GreaterThanOrEqualTo(0).WithMessage("Free delivery threshold cannot be negative")
            .GreaterThanOrEqualTo(x => x.MinOrderAmount).WithMessage("Free delivery threshold must be greater than or equal to minimum order amount");

        RuleFor(x => x.DefaultPreparationTimeMinutes)
            .InclusiveBetween(5, 300).WithMessage("Default preparation time must be between 5 and 300 minutes");

        RuleFor(x => x.OpeningTime)
            .NotEmpty().WithMessage("Opening time is required")
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$").WithMessage("Opening time must be in HH:mm format (e.g., 08:00)");

        RuleFor(x => x.ClosingTime)
            .NotEmpty().WithMessage("Closing time is required")
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$").WithMessage("Closing time must be in HH:mm format (e.g., 23:00)");
    }
}
