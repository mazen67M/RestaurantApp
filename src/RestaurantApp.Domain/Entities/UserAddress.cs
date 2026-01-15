namespace RestaurantApp.Domain.Entities;

public class UserAddress : BaseEntity
{
    public int UserId { get; set; }
    public string Label { get; set; } = string.Empty; // Home, Work, etc.
    public string AddressLine { get; set; } = string.Empty;
    public string? BuildingName { get; set; }
    public string? Floor { get; set; }
    public string? Apartment { get; set; }
    public string? Landmark { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsDefault { get; set; } = false;

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}
