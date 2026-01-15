using RestaurantApp.Application.Common;

namespace RestaurantApp.Application.DTOs.Favorite;

/// <summary>
/// Favorite item display DTO
/// </summary>
public record FavoriteDto(
    int Id,
    int MenuItemId,
    string NameEn,
    string NameAr,
    decimal Price,
    decimal? DiscountedPrice,
    string? ImageUrl,
    bool IsAvailable,
    DateTime AddedAt
);

/// <summary>
/// Add to favorites request
/// </summary>
public record AddFavoriteDto(
    int MenuItemId
);
