namespace RestaurantApp.Domain.Entities;

/// <summary>
/// Represents a delivery driver/person who can be assigned to orders
/// </summary>
public class Delivery : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    
    // Stats (denormalized for performance)
    public int TotalOrders { get; set; } = 0;
    public decimal TotalRevenue { get; set; } = 0;
    
    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
