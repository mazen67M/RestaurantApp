namespace RestaurantApp.Domain.Entities;

public class Branch : BaseEntity
{
    public int RestaurantId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string AddressAr { get; set; } = string.Empty;
    public string AddressEn { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Phone { get; set; }
    public decimal DeliveryRadiusKm { get; set; } = 10;
    public decimal MinOrderAmount { get; set; } = 0;
    public decimal DeliveryFee { get; set; } = 0;
    public decimal FreeDeliveryThreshold { get; set; } = 0;
    public int DefaultPreparationTimeMinutes { get; set; } = 30;
    public TimeSpan OpeningTime { get; set; } = new TimeSpan(9, 0, 0);
    public TimeSpan ClosingTime { get; set; } = new TimeSpan(23, 0, 0);
    public bool IsActive { get; set; } = true;
    public bool AcceptingOrders { get; set; } = true;

    // Navigation properties
    public virtual Restaurant Restaurant { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
