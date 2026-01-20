using FluentValidation;
using RestaurantApp.Application.DTOs.Restaurant;

namespace RestaurantApp.Application.Validators.Restaurant;

public class UpdateBranchDtoValidator : AbstractValidator<UpdateBranchDto>
{
    public UpdateBranchDtoValidator()
    {
        RuleFor(x => x.NameAr)
            .MaximumLength(200).WithMessage("Arabic name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.NameAr));

        RuleFor(x => x.NameEn)
            .MaximumLength(200).WithMessage("English name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.NameEn));

        RuleFor(x => x.AddressAr)
            .MaximumLength(500).WithMessage("Arabic address cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.AddressAr));

        RuleFor(x => x.AddressEn)
            .MaximumLength(500).WithMessage("English address cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.AddressEn));

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180")
            .When(x => x.Longitude.HasValue);

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.DeliveryRadiusKm)
            .GreaterThan(0).WithMessage("Delivery radius must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Delivery radius cannot exceed 100 km")
            .When(x => x.DeliveryRadiusKm.HasValue);

        RuleFor(x => x.MinOrderAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum order amount cannot be negative")
            .LessThanOrEqualTo(1000).WithMessage("Minimum order amount cannot exceed 1000")
            .When(x => x.MinOrderAmount.HasValue);

        RuleFor(x => x.DeliveryFee)
            .GreaterThanOrEqualTo(0).WithMessage("Delivery fee cannot be negative")
            .LessThanOrEqualTo(100).WithMessage("Delivery fee cannot exceed 100")
            .When(x => x.DeliveryFee.HasValue);

        RuleFor(x => x.FreeDeliveryThreshold)
            .GreaterThanOrEqualTo(0).WithMessage("Free delivery threshold cannot be negative")
            .When(x => x.FreeDeliveryThreshold.HasValue);

        RuleFor(x => x.DefaultPreparationTimeMinutes)
            .InclusiveBetween(5, 300).WithMessage("Default preparation time must be between 5 and 300 minutes")
            .When(x => x.DefaultPreparationTimeMinutes.HasValue);

        RuleFor(x => x.OpeningTime)
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$").WithMessage("Opening time must be in HH:mm format (e.g., 08:00)")
            .When(x => !string.IsNullOrEmpty(x.OpeningTime));

        RuleFor(x => x.ClosingTime)
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$").WithMessage("Closing time must be in HH:mm format (e.g., 23:00)")
            .When(x => !string.IsNullOrEmpty(x.ClosingTime));
    }
}
