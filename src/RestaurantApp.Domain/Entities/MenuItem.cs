namespace RestaurantApp.Domain.Entities;

public class MenuItem : BaseEntity
{
    public int CategoryId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string? ImageUrl { get; set; }
    public int PreparationTimeMinutes { get; set; } = 15;
    public int DisplayOrder { get; set; } = 0;
    public bool IsAvailable { get; set; } = true;
    public bool IsPopular { get; set; } = false;
    public int? Calories { get; set; }

    // Navigation properties
    public virtual MenuCategory Category { get; set; } = null!;
    public virtual ICollection<MenuItemAddOn> AddOns { get; set; } = new List<MenuItemAddOn>();
}
