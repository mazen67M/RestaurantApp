namespace RestaurantApp.Domain.Entities;

public class MenuItemAddOn : BaseEntity
{
    public int MenuItemId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;

    // Navigation properties
    public virtual MenuItem MenuItem { get; set; } = null!;
}
