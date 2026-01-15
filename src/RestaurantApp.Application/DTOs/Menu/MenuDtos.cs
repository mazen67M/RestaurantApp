using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Application.DTOs.Menu;

public record MenuCategoryDto(
    int Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    string? ImageUrl,
    int DisplayOrder,
    bool IsActive,
    int ItemCount
);

public record MenuItemDto(
    int Id,
    int CategoryId,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    decimal Price,
    decimal? DiscountedPrice,
    string? ImageUrl,
    int PreparationTimeMinutes,
    bool IsAvailable,
    bool IsPopular,
    int? Calories,
    List<MenuItemAddOnDto> AddOns
);

public record MenuItemSummaryDto(
    int Id,
    string NameAr,
    string NameEn,
    decimal Price,
    decimal? DiscountedPrice,
    string? ImageUrl,
    bool IsAvailable,
    bool IsPopular,
    int CategoryId = 0,
    string? CategoryName = null,
    string? DescriptionAr = null,
    string? DescriptionEn = null
);

public record MenuItemAddOnDto(
    int Id,
    string NameAr,
    string NameEn,
    decimal Price,
    bool IsAvailable
);

// Create/Update DTOs
public record CreateMenuCategoryDto(
    [property: Required(ErrorMessage = "Arabic name is required")]
    [property: StringLength(100, ErrorMessage = "Arabic name cannot exceed 100 characters")]
    string NameAr,
    
    [property: Required(ErrorMessage = "English name is required")]
    [property: StringLength(100, ErrorMessage = "English name cannot exceed 100 characters")]
    string NameEn,
    
    [property: StringLength(500, ErrorMessage = "Arabic description cannot exceed 500 characters")]
    string? DescriptionAr,
    
    [property: StringLength(500, ErrorMessage = "English description cannot exceed 500 characters")]
    string? DescriptionEn,
    
    [property: Url(ErrorMessage = "Invalid image URL")]
    [property: StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    string? ImageUrl,
    
    [property: Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000")]
    int DisplayOrder = 0
);

public record UpdateMenuCategoryDto(
    [property: Required(ErrorMessage = "Arabic name is required")]
    [property: StringLength(100, ErrorMessage = "Arabic name cannot exceed 100 characters")]
    string NameAr,
    
    [property: Required(ErrorMessage = "English name is required")]
    [property: StringLength(100, ErrorMessage = "English name cannot exceed 100 characters")]
    string NameEn,
    
    [property: StringLength(500, ErrorMessage = "Arabic description cannot exceed 500 characters")]
    string? DescriptionAr,
    
    [property: StringLength(500, ErrorMessage = "English description cannot exceed 500 characters")]
    string? DescriptionEn,
    
    [property: Url(ErrorMessage = "Invalid image URL")]
    [property: StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    string? ImageUrl,
    
    [property: Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000")]
    int DisplayOrder,
    
    bool IsActive
);

public record CreateMenuItemDto(
    [property: Required(ErrorMessage = "Category ID is required")]
    [property: Range(1, int.MaxValue, ErrorMessage = "Invalid category ID")]
    int CategoryId,
    
    [property: Required(ErrorMessage = "Arabic name is required")]
    [property: StringLength(100, ErrorMessage = "Arabic name cannot exceed 100 characters")]
    string NameAr,
    
    [property: Required(ErrorMessage = "English name is required")]
    [property: StringLength(100, ErrorMessage = "English name cannot exceed 100 characters")]
    string NameEn,
    
    [property: StringLength(1000, ErrorMessage = "Arabic description cannot exceed 1000 characters")]
    string? DescriptionAr,
    
    [property: StringLength(1000, ErrorMessage = "English description cannot exceed 1000 characters")]
    string? DescriptionEn,
    
    [property: Required(ErrorMessage = "Price is required")]
    [property: Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    decimal Price,
    
    [property: Range(0.01, 10000, ErrorMessage = "Discounted price must be between 0.01 and 10000")]
    decimal? DiscountedPrice,
    
    [property: Url(ErrorMessage = "Invalid image URL")]
    [property: StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    string? ImageUrl,
    
    [property: Range(1, 300, ErrorMessage = "Preparation time must be between 1 and 300 minutes")]
    int PreparationTimeMinutes = 15,
    
    [property: Range(0, 10000, ErrorMessage = "Calories must be between 0 and 10000")]
    int? Calories = null,
    
    List<CreateMenuItemAddOnDto>? AddOns = null
);

public record UpdateMenuItemDto(
    [property: Required(ErrorMessage = "Arabic name is required")]
    [property: StringLength(100, ErrorMessage = "Arabic name cannot exceed 100 characters")]
    string NameAr,
    
    [property: Required(ErrorMessage = "English name is required")]
    [property: StringLength(100, ErrorMessage = "English name cannot exceed 100 characters")]
    string NameEn,
    
    [property: StringLength(1000, ErrorMessage = "Arabic description cannot exceed 1000 characters")]
    string? DescriptionAr,
    
    [property: StringLength(1000, ErrorMessage = "English description cannot exceed 1000 characters")]
    string? DescriptionEn,
    
    [property: Required(ErrorMessage = "Price is required")]
    [property: Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    decimal Price,
    
    [property: Range(0.01, 10000, ErrorMessage = "Discounted price must be between 0.01 and 10000")]
    decimal? DiscountedPrice,
    
    [property: Url(ErrorMessage = "Invalid image URL")]
    [property: StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    string? ImageUrl,
    
    [property: Range(1, 300, ErrorMessage = "Preparation time must be between 1 and 300 minutes")]
    int PreparationTimeMinutes,
    
    bool IsAvailable,
    bool IsPopular,
    
    [property: Range(0, 10000, ErrorMessage = "Calories must be between 0 and 10000")]
    int? Calories
);

public record CreateMenuItemAddOnDto(
    [property: Required(ErrorMessage = "Arabic name is required")]
    [property: StringLength(100, ErrorMessage = "Arabic name cannot exceed 100 characters")]
    string NameAr,
    
    [property: Required(ErrorMessage = "English name is required")]
    [property: StringLength(100, ErrorMessage = "English name cannot exceed 100 characters")]
    string NameEn,
    
    [property: Required(ErrorMessage = "Price is required")]
    [property: Range(0.01, 1000, ErrorMessage = "Price must be between 0.01 and 1000")]
    decimal Price
);
