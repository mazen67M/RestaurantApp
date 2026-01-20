using FluentValidation;
using RestaurantApp.Application.DTOs.Review;

namespace RestaurantApp.Application.Validators.Review;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("Order ID is required and must be greater than 0");

        RuleFor(x => x.MenuItemId)
            .GreaterThan(0).WithMessage("Menu item ID is required and must be greater than 0");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}
