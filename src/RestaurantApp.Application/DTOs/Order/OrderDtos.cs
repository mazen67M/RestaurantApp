using System.ComponentModel.DataAnnotations;
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.Application.DTOs.Order;

public record OrderDto(
    int Id,
    string OrderNumber,
    OrderType OrderType,
    OrderStatus Status,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    decimal SubTotal,
    decimal DeliveryFee,
    decimal Discount,
    decimal Total,
    string? DeliveryAddressLine,
    string? CustomerNotes,
    DateTime? RequestedDeliveryTime,
    DateTime? EstimatedDeliveryTime,
    DateTime CreatedAt,
    BranchOrderInfo Branch,
    List<OrderItemDto> Items
);

public record BranchOrderInfo(
    int Id,
    string NameAr,
    string NameEn,
    string? Phone
);

public record OrderItemDto(
    int Id,
    int MenuItemId,
    string MenuItemNameAr,
    string MenuItemNameEn,
    decimal UnitPrice,
    int Quantity,
    decimal AddOnsTotal,
    decimal TotalPrice,
    string? Notes,
    List<OrderItemAddOnDto> AddOns
);

public record OrderItemAddOnDto(
    int Id,
    string NameAr,
    string NameEn,
    decimal Price
);

public record OrderSummaryDto(
    int Id,
    string OrderNumber,
    OrderStatus Status,
    decimal Total,
    int ItemCount,
    DateTime CreatedAt,
    string BranchNameAr,
    string BranchNameEn,
    string CustomerName,
    string CustomerPhone
);

public record OrderTrackingDto(
    string OrderNumber,
    OrderStatus Status,
    DateTime? EstimatedDeliveryTime,
    List<OrderStatusHistoryDto> StatusHistory
);

public record OrderStatusHistoryDto(
    OrderStatus Status,
    DateTime Timestamp
);

// Create Order DTOs
public record CreateOrderDto(
    [Required(ErrorMessage = "Branch ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid branch ID")]
    int BranchId,
    
    [Required]
    OrderType OrderType,
    
    int? AddressId,
    
    [StringLength(500, ErrorMessage = "Delivery address cannot exceed 500 characters")]
    string? DeliveryAddressLine,
    
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    decimal? DeliveryLatitude,
    
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    decimal? DeliveryLongitude,
    
    [StringLength(500, ErrorMessage = "Delivery notes cannot exceed 500 characters")]
    string? DeliveryNotes,
    
    DateTime? RequestedDeliveryTime,
    
    [StringLength(1000, ErrorMessage = "Customer notes cannot exceed 1000 characters")]
    string? CustomerNotes,
    
    [StringLength(50, ErrorMessage = "Coupon code cannot exceed 50 characters")]
    string? CouponCode,
    
    [Required(ErrorMessage = "Order must have at least one item")]
    [MinLength(1, ErrorMessage = "Order must have at least one item")]
    List<CreateOrderItemDto> Items
);

public record CreateOrderItemDto(
    [Required(ErrorMessage = "Menu item ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid menu item ID")]
    int MenuItemId,
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    int Quantity,
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    string? Notes,
    
    List<int>? AddOnIds
);

public record OrderCreatedDto(
    int OrderId,
    string OrderNumber,
    decimal Total,
    DateTime EstimatedDeliveryTime
);
