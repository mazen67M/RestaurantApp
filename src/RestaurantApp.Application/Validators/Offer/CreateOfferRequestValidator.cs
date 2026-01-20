using FluentValidation;
using RestaurantApp.Application.DTOs.Offer;

namespace RestaurantApp.Application.Validators.Offer;

public class CreateOfferRequestValidator : AbstractValidator<CreateOfferRequest>
{
    public CreateOfferRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Offer code is required")
            .MaximumLength(50).WithMessage("Offer code cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9_-]+$").WithMessage("Offer code must contain only uppercase letters, numbers, hyphens, and underscores");

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required")
            .MaximumLength(200).WithMessage("Arabic name cannot exceed 200 characters");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required")
            .MaximumLength(200).WithMessage("English name cannot exceed 200 characters");

        RuleFor(x => x.DescriptionAr)
            .MaximumLength(1000).WithMessage("Arabic description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.DescriptionAr));

        RuleFor(x => x.DescriptionEn)
            .MaximumLength(1000).WithMessage("English description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.DescriptionEn));

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Offer type is required")
            .Must(type => new[] { "Percentage", "FixedAmount", "FreeDelivery", "BuyOneGetOne" }.Contains(type))
            .WithMessage("Offer type must be one of: Percentage, FixedAmount, FreeDelivery, BuyOneGetOne");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Offer value must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Percentage discount cannot exceed 100%")
            .When(x => x.Type == "Percentage");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Offer value must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Fixed discount cannot exceed 1000")
            .When(x => x.Type == "FixedAmount");

        RuleFor(x => x.MinimumOrderAmount)
            .GreaterThan(0).WithMessage("Minimum order amount must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Minimum order amount cannot exceed 10000")
            .When(x => x.MinimumOrderAmount.HasValue);

        RuleFor(x => x.MaximumDiscount)
            .GreaterThan(0).WithMessage("Maximum discount must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Maximum discount cannot exceed 1000")
            .When(x => x.MaximumDiscount.HasValue);

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("End date must be in the future");

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0).WithMessage("Usage limit must be greater than 0")
            .LessThanOrEqualTo(1000000).WithMessage("Usage limit cannot exceed 1,000,000")
            .When(x => x.UsageLimit.HasValue);

        RuleFor(x => x.PerUserLimit)
            .GreaterThan(0).WithMessage("Per user limit must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Per user limit cannot exceed 100")
            .When(x => x.PerUserLimit.HasValue);

        RuleFor(x => x.BranchId)
            .GreaterThan(0).WithMessage("Invalid branch ID")
            .When(x => x.BranchId.HasValue);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Invalid category ID")
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.MenuItemId)
            .GreaterThan(0).WithMessage("Invalid menu item ID")
            .When(x => x.MenuItemId.HasValue);
    }
}
