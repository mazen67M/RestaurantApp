namespace RestaurantApp.Application.DTOs.Delivery;

public record DeliveryDto(
    int Id,
    string NameAr,
    string NameEn,
    string Phone,
    string? Email,
    bool IsActive,
    bool IsAvailable,
    int TotalOrders,
    decimal TotalRevenue,
    DateTime CreatedAt
);

public record CreateDeliveryDto(
    string NameAr,
    string NameEn,
    string Phone,
    string? Email,
    bool IsActive = true,
    bool IsAvailable = true
);

public record UpdateDeliveryDto(
    string? NameAr,
    string? NameEn,
    string? Phone,
    string? Email,
    bool? IsActive,
    bool? IsAvailable
);

public record DeliveryStatsDto(
    int DeliveryId,
    string DeliveryName,
    int TotalOrders,
    decimal TotalRevenue,
    int OrdersToday,
    decimal RevenueToday,
    int OrdersThisWeek,
    decimal RevenueThisWeek,
    int OrdersThisMonth,
    decimal RevenueThisMonth
);

public record AssignDeliveryDto(
    int DeliveryId
);
