using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Application.DTOs.Review;

/// <summary>
/// Review display DTO
/// </summary>
public record ReviewDto(
    int Id,
    int MenuItemId,
    string MenuItemName,
    int OrderId,
    string CustomerName,
    int Rating,
    string? Comment,
    bool IsApproved,
    DateTime CreatedAt
);

/// <summary>
/// Review summary for display on menu items
/// </summary>
public record ReviewSummaryDto(
    int MenuItemId,
    double AverageRating,
    int TotalReviews,
    int FiveStarCount,
    int FourStarCount,
    int ThreeStarCount,
    int TwoStarCount,
    int OneStarCount
);

/// <summary>
/// Create a new review
/// </summary>
public record CreateReviewDto(
    [property: Required(ErrorMessage = "Order ID is required")]
    [property: Range(1, int.MaxValue, ErrorMessage = "Invalid order ID")]
    int OrderId,
    
    [property: Required(ErrorMessage = "Menu item ID is required")]
    [property: Range(1, int.MaxValue, ErrorMessage = "Invalid menu item ID")]
    int MenuItemId,
    
    [property: Required(ErrorMessage = "Rating is required")]
    [property: Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    int Rating,
    
    [property: StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
    string? Comment
);

/// <summary>
/// Update existing review
/// </summary>
public record UpdateReviewDto(
    [property: Required(ErrorMessage = "Rating is required")]
    [property: Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    int Rating,
    
    [property: StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
    string? Comment
);

/// <summary>
/// Admin review moderation
/// </summary>
public record ReviewModerationDto(
    int Id,
    int MenuItemId,
    string MenuItemNameEn,
    string MenuItemNameAr,
    int OrderId,
    string CustomerName,
    string CustomerEmail,
    int Rating,
    string? Comment,
    bool IsApproved,
    bool IsVisible,
    DateTime CreatedAt
);
