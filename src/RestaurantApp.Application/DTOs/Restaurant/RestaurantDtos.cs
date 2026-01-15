namespace RestaurantApp.Application.DTOs.Restaurant;

public record RestaurantDto(
    int Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    string? LogoUrl,
    string? CoverImageUrl,
    string PrimaryColor,
    string SecondaryColor,
    string? Phone,
    string? Email,
    bool IsActive
);

public record BranchDto(
    int Id,
    int RestaurantId,
    string NameAr,
    string NameEn,
    string AddressAr,
    string AddressEn,
    decimal Latitude,
    decimal Longitude,
    string? Phone,
    decimal DeliveryRadiusKm,
    decimal MinOrderAmount,
    decimal DeliveryFee,
    decimal FreeDeliveryThreshold,
    int DefaultPreparationTimeMinutes,
    string OpeningTime,  // Changed from TimeSpan to string (format: "HH:mm")
    string ClosingTime,  // Changed from TimeSpan to string (format: "HH:mm")
    bool IsActive,
    bool AcceptingOrders,
    double? DistanceKm = null
);

public record BranchSummaryDto(
    int Id,
    string NameAr,
    string NameEn,
    string AddressAr,
    string AddressEn,
    bool IsActive,
    bool AcceptingOrders,
    double? DistanceKm = null
);

public record CreateBranchDto(
    string NameAr,
    string NameEn,
    string AddressAr,
    string AddressEn,
    decimal Latitude,
    decimal Longitude,
    string? Phone,
    decimal DeliveryRadiusKm,
    decimal MinOrderAmount,
    decimal DeliveryFee,
    decimal FreeDeliveryThreshold,
    int DefaultPreparationTimeMinutes,
    string OpeningTime,  // Format: "HH:mm" e.g., "08:00"
    string ClosingTime,  // Format: "HH:mm" e.g., "23:00"
    bool IsActive = true,
    bool AcceptingOrders = true
);

public record UpdateBranchDto(
    string? NameAr,
    string? NameEn,
    string? AddressAr,
    string? AddressEn,
    decimal? Latitude,
    decimal? Longitude,
    string? Phone,
    decimal? DeliveryRadiusKm,
    decimal? MinOrderAmount,
    decimal? DeliveryFee,
    decimal? FreeDeliveryThreshold,
    int? DefaultPreparationTimeMinutes,
    string? OpeningTime,  // Format: "HH:mm" e.g., "08:00"
    string? ClosingTime,  // Format: "HH:mm" e.g., "23:00"
    bool? IsActive,
    bool? AcceptingOrders
);

public record UpdateRestaurantDto(
    string? NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    string? LogoUrl,
    string? CoverImageUrl,
    string? PrimaryColor,
    string? SecondaryColor,
    string? Phone,
    string? Email,
    bool? IsActive
);
