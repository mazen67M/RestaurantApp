using RestaurantApp.Domain.Enums;

namespace RestaurantApp.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public int BranchId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderType OrderType { get; set; } = OrderType.Delivery;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    
    // Pricing
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Discount { get; set; } = 0;
    public string? CouponCode { get; set; }  // Promo code used for this order
    public decimal Total { get; set; }
    
    // Delivery details
    public string? DeliveryAddressLine { get; set; }
    public decimal? DeliveryLatitude { get; set; }
    public decimal? DeliveryLongitude { get; set; }
    public string? DeliveryNotes { get; set; }
    
    // Timing
    public DateTime? RequestedDeliveryTime { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    
    // Delivery Driver
    public int? DeliveryId { get; set; }
    public DateTime? AssignedToDeliveryAt { get; set; }
    
    // Legacy field (deprecated - use DeliveryId instead)
    public int? DriverId { get; set; }
    
    // Notes
    public string? CustomerNotes { get; set; }
    public string? CancellationReason { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual Delivery? Delivery { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

