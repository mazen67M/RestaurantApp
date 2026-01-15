using RestaurantApp.Domain.Enums;

namespace RestaurantApp.Domain.Entities;

/// <summary>
/// Tracks the history of status changes for an order
/// </summary>
public class OrderStatusHistory : BaseEntity
{
    /// <summary>
    /// The order this history entry belongs to
    /// </summary>
    public int OrderId { get; set; }
    
    /// <summary>
    /// The previous status before this change
    /// </summary>
    public OrderStatus? PreviousStatus { get; set; }
    
    /// <summary>
    /// The new status after this change
    /// </summary>
    public OrderStatus NewStatus { get; set; }
    
    /// <summary>
    /// Who made this status change (user ID or system)
    /// </summary>
    public string? ChangedBy { get; set; }
    
    /// <summary>
    /// Optional notes about the status change
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// When this status change occurred (inherited from BaseEntity.CreatedAt)
    /// </summary>
    // CreatedAt from BaseEntity serves as the timestamp
    
    /// <summary>
    /// Navigation property to the order
    /// </summary>
    public virtual Order Order { get; set; } = null!;
}
