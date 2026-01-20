using FluentValidation;
using RestaurantApp.Application.DTOs.Menu;

namespace RestaurantApp.Application.Validators.Menu;

public class CreateMenuCategoryDtoValidator : AbstractValidator<CreateMenuCategoryDto>
{
    public CreateMenuCategoryDtoValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required")
            .MaximumLength(100).WithMessage("Arabic name cannot exceed 100 characters");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required")
            .MaximumLength(100).WithMessage("English name cannot exceed 100 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater");
    }
}

public class UpdateMenuCategoryDtoValidator : AbstractValidator<UpdateMenuCategoryDto>
{
    public UpdateMenuCategoryDtoValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required")
            .MaximumLength(100).WithMessage("Arabic name cannot exceed 100 characters");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required")
            .MaximumLength(100).WithMessage("English name cannot exceed 100 characters");
    }
}

public class CreateMenuItemDtoValidator : AbstractValidator<CreateMenuItemDto>
{
    public CreateMenuItemDtoValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Valid category is required");

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required")
            .MaximumLength(200).WithMessage("Arabic name cannot exceed 200 characters");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required")
            .MaximumLength(200).WithMessage("English name cannot exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.PreparationTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Prep time cannot be negative");
    }
}

public class UpdateMenuItemDtoValidator : AbstractValidator<UpdateMenuItemDto>
{
    public UpdateMenuItemDtoValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required")
            .MaximumLength(200).WithMessage("Arabic name cannot exceed 200 characters");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required")
            .MaximumLength(200).WithMessage("English name cannot exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");
            
        RuleFor(x => x.PreparationTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Prep time cannot be negative");
    }
}
